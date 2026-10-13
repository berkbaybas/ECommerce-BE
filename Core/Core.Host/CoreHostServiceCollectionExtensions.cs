using Asp.Versioning;
using Core.Auth.Options;
using Core.Host.Hosting;
using Core.Host.Middleware;
using Core.IOC;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace Core.Host;

public static class CoreHostServiceCollectionExtensions
{
    private const string OpenIdConnectSchemeId = "oidc";

    public static IServiceCollection AddCoreHost(this IServiceCollection services, ServiceInfo serviceInfo, params Assembly[] assemblies)
    {
        services.AddIOC(assemblies);

        services.AddControllers();

        services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;
            })
            .AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = serviceInfo.Name,
                Description = serviceInfo.Description,
                Version = serviceInfo.Version,
            });

            options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = JwtBearerDefaults.AuthenticationScheme.ToLowerInvariant(),
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Paste an access token issued by the OAuth provider.",
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = JwtBearerDefaults.AuthenticationScheme,
                        },
                    },
                    Array.Empty<string>()
                },
            });
        });

        // Interactive authorization_code + PKCE login in Swagger UI, driven by the provider's OIDC discovery document.
        services.AddOptions<SwaggerGenOptions>()
            .Configure<IOptions<OAuthOptions>>((options, oAuthOptions) =>
            {
                if (string.IsNullOrWhiteSpace(oAuthOptions.Value.SwaggerClientId))
                {
                    return;
                }

                options.AddSecurityDefinition(OpenIdConnectSchemeId, new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OpenIdConnect,
                    OpenIdConnectUrl = new Uri($"{oAuthOptions.Value.Authority.TrimEnd('/')}/.well-known/openid-configuration"),
                    Description = "Log in via the OAuth provider (authorization_code + PKCE).",
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = OpenIdConnectSchemeId,
                            },
                        },
                        Array.Empty<string>()
                    },
                });
            });

        return services;
    }

    public static WebApplication UseCoreHost(this WebApplication app)
    {
        app.UseCoreExceptionHandling();

        var oAuthOptions = app.Services.GetRequiredService<IOptions<OAuthOptions>>().Value;

        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            if (string.IsNullOrWhiteSpace(oAuthOptions.SwaggerClientId))
            {
                return;
            }

            options.OAuthClientId(oAuthOptions.SwaggerClientId);
            options.OAuthUsePkce();
        });

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        return app;
    }
}
