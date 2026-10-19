using Core.Auth.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Core.Auth;

public static class CoreAuthServiceCollectionExtensions
{
    public static IServiceCollection AddCoreAuth(this IServiceCollection services, IConfiguration configuration)
    {
        var section = configuration.GetSection(OAuthOptions.SectionName);
        var oAuthOptions = section.Get<OAuthOptions>() ?? new OAuthOptions();
        services.Configure<OAuthOptions>(section);

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = oAuthOptions.Authority;
                if (!string.IsNullOrWhiteSpace(oAuthOptions.MetadataAddress))
                {
                    options.MetadataAddress = oAuthOptions.MetadataAddress;
                }

                options.Audience = oAuthOptions.Audience;
                options.RequireHttpsMetadata = oAuthOptions.RequireHttpsMetadata;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                };

                // Browsers cannot set an Authorization header on a WebSocket handshake, so SignalR
                // sends the token as ?access_token=; accept it for hub paths only.
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];
                        if (!string.IsNullOrEmpty(accessToken) && context.HttpContext.Request.Path.StartsWithSegments("/hubs"))
                        {
                            context.Token = accessToken;
                        }

                        return Task.CompletedTask;
                    },
                };
            });

        return services;
    }
}
