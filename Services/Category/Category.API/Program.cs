using System.Reflection;
using Core.Auth;
using Core.Auth.Policies;
using Core.CQRS;
using Core.Data.Mongo;
using Core.Host;
using Core.Host.Hosting;
using Core.Host.Logging;
using Core.Validation;
using Category.API.Authorization;
using Category.Application.Categories;
using Category.Infrastructure.Repositories;

var serviceInfo = new ServiceInfo("Category Service", "E-commerce categories", "1.0.0");
var builder = WebApplication.CreateBuilder(args);
builder.AddCoreSerilog(serviceInfo);
var assemblies = new[] { Assembly.GetExecutingAssembly(), typeof(CreateCategoryCommand).Assembly, typeof(ICategoryRepository).Assembly };
builder.Services.AddCoreHost(serviceInfo, assemblies);
builder.Services.AddCoreCQRS(assemblies);
builder.Services.AddCoreValidation(assemblies);
builder.Services.AddCoreDataMongo(builder.Configuration);
builder.Services.AddCoreAuth(builder.Configuration);
builder.Services.AddAuthorizationBuilder()
    .AddPolicy(CategoryScopes.Read, policy => policy.RequireScope(CategoryScopes.Read))
    .AddPolicy(CategoryScopes.Write, policy => policy.RequireScope(CategoryScopes.Write));
var app = builder.Build();
app.UseCoreHost();
app.Run();
