using System.Net.Http.Headers;
using System.Net.WebSockets;
using Core.Auth;
using Core.Auth.Policies;
using Microsoft.OpenApi.Models;
using Asp.Versioning;
using Core.CQRS;
using BFF.Application.Products;
using BFF.Application.RemoteCall;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCoreAuth(builder.Configuration);
builder.Services.AddControllers();
builder.Services.AddApiVersioning(options => options.ReportApiVersions = true)
    .AddMvc().AddApiExplorer(options => { options.GroupNameFormat = "'v'VVV"; options.SubstituteApiVersionInUrl = true; });
builder.Services.AddCoreCQRS(typeof(ListProductsQuery).Assembly);
builder.Services.AddHttpContextAccessor();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "ECommerce BFF API", Version = "v1" });
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http, Scheme = "bearer", BearerFormat = "JWT",
        Description = "Enter an access token for the BFF and the requested services."
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        [new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" } }] = Array.Empty<string>()
    });
});
var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];
builder.Services.AddCors(options => options.AddDefaultPolicy(policy =>
    policy.WithOrigins(allowedOrigins).AllowAnyHeader().AllowAnyMethod()));
builder.Services.AddAuthorizationBuilder()
    .AddPolicy("products.read", policy => policy.RequireAuthenticatedUser().RequireScope("products.read"))
    .AddPolicy("products.write", policy => policy.RequireAuthenticatedUser().RequireScope("products.write"))
    .AddPolicy("categories.read", policy => policy.RequireAuthenticatedUser().RequireScope("categories.read"))
    .AddPolicy("categories.write", policy => policy.RequireAuthenticatedUser().RequireScope("categories.write"))
    .AddPolicy("catalog.read", policy => policy.RequireAuthenticatedUser().RequireScope("products.read").RequireScope("categories.read"));

foreach (var service in new[] { "Product", "Category" })
{
    var address = builder.Configuration[$"Services:{service}"];
    if (!Uri.TryCreate(address, UriKind.Absolute, out var uri) ||
        (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
        throw new InvalidOperationException($"Services:{service} must be an absolute HTTP(S) URL.");
    builder.Services.AddHttpClient(service, client => client.BaseAddress = uri)
        .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler { AllowAutoRedirect = false, UseCookies = false });
}
builder.Services.AddHttpClient<IProductRemoteCallService, ProductRemoteCallService>(client =>
    client.BaseAddress = new Uri(builder.Configuration["Services:Product"]!))
    .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler { AllowAutoRedirect = false, UseCookies = false });
builder.Services.AddHttpClient<ICategoryRemoteCallService, CategoryRemoteCallService>(client =>
    client.BaseAddress = new Uri(builder.Configuration["Services:Category"]!))
    .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler { AllowAutoRedirect = false, UseCookies = false });

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
app.UseWebSockets();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// SignalR uses POST to negotiate and may use GET/POST/DELETE for its transports.
app.MapMethods("/hubs/products/{**path}", ["GET", "POST", "DELETE"], async (HttpContext context, IHttpClientFactory clients) =>
{
    if (context.WebSockets.IsWebSocketRequest)
        await ForwardWebSocket(context, clients);
    else
        await Forward(context, clients, "Product");
}).RequireAuthorization("products.read").ExcludeFromDescription();

app.MapGet("/demo/index.html", (HttpContext context, IHttpClientFactory clients) => Forward(context, clients, "Product"))
    .ExcludeFromDescription();

app.Run();

static HttpRequestMessage MakeRequest(HttpContext context, string path)
{
    var request = new HttpRequestMessage(new HttpMethod(context.Request.Method), path);
    if (AuthenticationHeaderValue.TryParse(context.Request.Headers.Authorization, out var authorization))
        request.Headers.Authorization = authorization;
    return request;
}

static async Task Forward(HttpContext context, IHttpClientFactory clients, string service)
{
    try
    {
        // The destination host is fixed in configuration; only the incoming path and query are forwarded.
        var path = context.Request.Path.Value!.TrimStart('/') + context.Request.QueryString;
        using var request = MakeRequest(context, path);
        if (context.Request.ContentLength > 0 || context.Request.Headers.ContainsKey("Transfer-Encoding"))
        {
            request.Content = new StreamContent(context.Request.Body);
            if (context.Request.ContentType is { } contentType)
                request.Content.Headers.TryAddWithoutValidation("Content-Type", contentType);
        }
        using var response = await clients.CreateClient(service).SendAsync(request, HttpCompletionOption.ResponseHeadersRead, context.RequestAborted);
        context.Response.StatusCode = (int)response.StatusCode;
        if (response.Content.Headers.ContentType is { } responseType)
            context.Response.ContentType = responseType.ToString();
        await response.Content.CopyToAsync(context.Response.Body, context.RequestAborted);
    }
    catch (HttpRequestException)
    {
        if (!context.Response.HasStarted)
            await Results.Problem("The requested service is unavailable.", statusCode: 502).ExecuteAsync(context);
    }
    catch (OperationCanceledException) when (!context.RequestAborted.IsCancellationRequested)
    {
        if (!context.Response.HasStarted)
            await Results.Problem("The requested service timed out.", statusCode: 504).ExecuteAsync(context);
    }
}

static async Task ForwardWebSocket(HttpContext context, IHttpClientFactory clients)
{
    var baseAddress = clients.CreateClient("Product").BaseAddress!;
    var target = new UriBuilder(new Uri(baseAddress, context.Request.Path.Value!.TrimStart('/') + context.Request.QueryString))
    {
        Scheme = baseAddress.Scheme == Uri.UriSchemeHttps ? "wss" : "ws",
    };
    using var upstream = new ClientWebSocket();
    // SignalR browser clients use access_token in the query; non-browser clients may use a header.
    if (context.Request.Headers.Authorization.Count > 0)
        upstream.Options.SetRequestHeader("Authorization", context.Request.Headers.Authorization.ToString());
    try
    {
        await upstream.ConnectAsync(target.Uri, context.RequestAborted);
        using var downstream = await context.WebSockets.AcceptWebSocketAsync();
        using var cancellation = CancellationTokenSource.CreateLinkedTokenSource(context.RequestAborted);
        var outgoing = CopyWebSocket(downstream, upstream, cancellation.Token);
        var incoming = CopyWebSocket(upstream, downstream, cancellation.Token);
        await Task.WhenAny(outgoing, incoming);
        await cancellation.CancelAsync();
        try { await Task.WhenAll(outgoing, incoming); }
        catch (OperationCanceledException) when (cancellation.IsCancellationRequested) { }
    }
    catch (WebSocketException)
    {
        if (!context.Response.HasStarted)
            await Results.Problem("The product service is unavailable.", statusCode: 502).ExecuteAsync(context);
    }
}

static async Task CopyWebSocket(WebSocket source, WebSocket destination, CancellationToken cancellationToken)
{
    var buffer = new byte[16 * 1024];
    while (source.State == WebSocketState.Open && destination.State == WebSocketState.Open)
    {
        var result = await source.ReceiveAsync(buffer.AsMemory(), cancellationToken);
        if (result.MessageType == WebSocketMessageType.Close)
        {
            await destination.CloseOutputAsync(source.CloseStatus ?? WebSocketCloseStatus.NormalClosure,
                source.CloseStatusDescription, cancellationToken);
            return;
        }
        await destination.SendAsync(buffer.AsMemory(0, result.Count), result.MessageType, result.EndOfMessage, cancellationToken);
    }
}
