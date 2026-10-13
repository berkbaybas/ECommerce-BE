using System.Reflection;
using Core.Abstraction.Lifetime;
using Microsoft.Extensions.DependencyInjection;

namespace Core.IOC;

public static class CoreIOCServiceCollectionExtensions
{
    public static IServiceCollection AddIOC(this IServiceCollection services, params Assembly[] assemblies)
    {
        services.Scan(scan => scan.FromAssemblies(assemblies)
            .AddClasses(classes => classes.AssignableTo<IScopedService>(), publicOnly: false)
                .AsSelf()
                .AsImplementedInterfaces()
                .WithScopedLifetime()
            .AddClasses(classes => classes.AssignableTo<ISingletonService>(), publicOnly: false)
                .AsSelf()
                .AsImplementedInterfaces()
                .WithSingletonLifetime()
            .AddClasses(classes => classes.AssignableTo<ITransientService>(), publicOnly: false)
                .AsSelf()
                .AsImplementedInterfaces()
                .WithTransientLifetime());

        return services;
    }
}
