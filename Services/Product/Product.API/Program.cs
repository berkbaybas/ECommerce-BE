using System.Reflection;
using Core.Auth;
using Core.Auth.Policies;
using Core.CQRS;
using Core.Data.Mongo;
using Core.Host;
using Core.Host.Hosting;
using Core.Host.Logging;
using Core.Realtime;
using Core.Validation;
using Product.API.Authorization;
using Product.Application.Realtime;

var serviceInfo = new ServiceInfo("Product Service", "E-commerce product catalog with real-time stock/price updates", "1.0.0");

var builder = WebApplication.CreateBuilder(args);
builder.AddCoreSerilog(serviceInfo);

var applicationAssemblies = new[]
{
    Assembly.GetExecutingAssembly(),
    typeof(Product.Application.Products.CreateProductCommand).Assembly,
    typeof(Product.Infrastructure.Repositories.IProductRepository).Assembly,
};

builder.Services.AddCoreHost(serviceInfo, applicationAssemblies);
builder.Services.AddCoreCQRS(applicationAssemblies);
builder.Services.AddCoreValidation(applicationAssemblies);
builder.Services.AddCoreDataMongo(builder.Configuration);
builder.Services.AddCoreRealtime();
builder.Services.AddCoreAuth(builder.Configuration);

builder.Services.AddAuthorizationBuilder()
    .AddPolicy(ProductScopes.Read, policy => policy.RequireScope(ProductScopes.Read))
    .AddPolicy(ProductScopes.Write, policy => policy.RequireScope(ProductScopes.Write));

var app = builder.Build();

app.UseStaticFiles();
app.UseCoreHost();
app.MapHub<ProductHub>("/hubs/products").RequireAuthorization(ProductScopes.Read);

app.Run();
