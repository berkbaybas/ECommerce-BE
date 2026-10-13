using Core.Realtime.Notifiers;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Realtime;

public static class CoreRealtimeServiceCollectionExtensions
{
    public static IServiceCollection AddCoreRealtime(this IServiceCollection services)
    {
        services.AddSignalR();
        services.AddScoped(typeof(IRealtimeNotifier<>), typeof(RealtimeNotifier<>));

        return services;
    }
}
