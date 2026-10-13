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
                options.Audience = oAuthOptions.Audience;
                options.RequireHttpsMetadata = oAuthOptions.RequireHttpsMetadata;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                };
            });

        return services;
    }
}
