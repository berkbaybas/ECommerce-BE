using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Http;
using Product.Abstraction.Requests.Products;
using Category.Abstraction;

namespace BFF.Application.RemoteCall;

public sealed record RemoteResponse(int StatusCode, byte[] Body, string? ContentType);

public interface IProductRemoteCallService
{
    Task<RemoteResponse> List(CancellationToken ct);
    Task<RemoteResponse> Search(string queryString, CancellationToken ct);
    Task<RemoteResponse> Find(string id, CancellationToken ct);
    Task<RemoteResponse> Create(CreateProductRequest request, CancellationToken ct);
    Task<RemoteResponse> UpdateDetails(string id, UpdateProductDetailsRequest request, CancellationToken ct);
    Task<RemoteResponse> UpdateStock(string id, UpdateProductStockRequest request, CancellationToken ct);
    Task<RemoteResponse> UpdatePrice(string id, UpdateProductPriceRequest request, CancellationToken ct);
    Task<RemoteResponse> Delete(string id, CancellationToken ct);
}

public interface ICategoryRemoteCallService
{
    Task<RemoteResponse> List(CancellationToken ct);
    Task<RemoteResponse> Find(string id, CancellationToken ct);
    Task<RemoteResponse> Create(CategoryRequest request, CancellationToken ct);
    Task<RemoteResponse> Update(string id, CategoryRequest request, CancellationToken ct);
    Task<RemoteResponse> Delete(string id, CancellationToken ct);
}

public abstract class ServiceClient(HttpClient client, IHttpContextAccessor contextAccessor)
{
    protected async Task<RemoteResponse> SendAsync(HttpMethod method, string path, object? body, CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(method, path);
        if (AuthenticationHeaderValue.TryParse(contextAccessor.HttpContext?.Request.Headers.Authorization, out var authorization))
            request.Headers.Authorization = authorization;
        if (body is not null) request.Content = JsonContent.Create(body, body.GetType());
        try
        {
            using var response = await client.SendAsync(request, cancellationToken);
            return new RemoteResponse((int)response.StatusCode, await response.Content.ReadAsByteArrayAsync(cancellationToken), response.Content.Headers.ContentType?.ToString());
        }
        catch (HttpRequestException)
        {
            return new RemoteResponse(502, "{\"title\":\"The requested service is unavailable.\",\"status\":502}"u8.ToArray(), "application/problem+json");
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            return new RemoteResponse(504, "{\"title\":\"The requested service timed out.\",\"status\":504}"u8.ToArray(), "application/problem+json");
        }
    }
}

public sealed class ProductRemoteCallService(HttpClient client, IHttpContextAccessor context) : ServiceClient(client, context), IProductRemoteCallService
{
    private static string Item(string id) => $"api/v1/products/{Uri.EscapeDataString(id)}";
    public Task<RemoteResponse> List(CancellationToken ct) => SendAsync(HttpMethod.Get, "api/v1/products", null, ct);
    public Task<RemoteResponse> Search(string queryString, CancellationToken ct) => SendAsync(HttpMethod.Get, "api/v1/products/search" + queryString, null, ct);
    public Task<RemoteResponse> Find(string id, CancellationToken ct) => SendAsync(HttpMethod.Get, Item(id), null, ct);
    public Task<RemoteResponse> Create(CreateProductRequest request, CancellationToken ct) => SendAsync(HttpMethod.Post, "api/v1/products", request, ct);
    public Task<RemoteResponse> UpdateDetails(string id, UpdateProductDetailsRequest request, CancellationToken ct) => SendAsync(HttpMethod.Put, Item(id), request, ct);
    public Task<RemoteResponse> UpdateStock(string id, UpdateProductStockRequest request, CancellationToken ct) => SendAsync(HttpMethod.Patch, Item(id) + "/stock", request, ct);
    public Task<RemoteResponse> UpdatePrice(string id, UpdateProductPriceRequest request, CancellationToken ct) => SendAsync(HttpMethod.Patch, Item(id) + "/price", request, ct);
    public Task<RemoteResponse> Delete(string id, CancellationToken ct) => SendAsync(HttpMethod.Delete, Item(id), null, ct);
}

public sealed class CategoryRemoteCallService(HttpClient client, IHttpContextAccessor context) : ServiceClient(client, context), ICategoryRemoteCallService
{
    private static string Item(string id) => $"api/v1/categories/{Uri.EscapeDataString(id)}";
    public Task<RemoteResponse> List(CancellationToken ct) => SendAsync(HttpMethod.Get, "api/v1/categories", null, ct);
    public Task<RemoteResponse> Find(string id, CancellationToken ct) => SendAsync(HttpMethod.Get, Item(id), null, ct);
    public Task<RemoteResponse> Create(CategoryRequest request, CancellationToken ct) => SendAsync(HttpMethod.Post, "api/v1/categories", request, ct);
    public Task<RemoteResponse> Update(string id, CategoryRequest request, CancellationToken ct) => SendAsync(HttpMethod.Put, Item(id), request, ct);
    public Task<RemoteResponse> Delete(string id, CancellationToken ct) => SendAsync(HttpMethod.Delete, Item(id), null, ct);
}
