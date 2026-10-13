using System.Reflection;
using Core.CQRS.Behaviors;
using Core.CQRS.Processing;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Core.CQRS;

public static class CoreCQRSServiceCollectionExtensions
{
    public static IServiceCollection AddCoreCQRS(this IServiceCollection services, params Assembly[] assemblies)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblies(assemblies);
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        services.AddScoped<ICQRSProcessor, CQRSProcessor>();

        return services;
    }
}
