using Microsoft.AspNetCore.SignalR;

namespace Core.Realtime.Notifiers;

/// <summary>Generic real-time broadcaster over a SignalR hub — payload/group semantics stay with the caller.</summary>
public interface IRealtimeNotifier<THub>
    where THub : Hub
{
    Task BroadcastAsync<TPayload>(string method, TPayload payload, CancellationToken cancellationToken);

    Task NotifyGroupAsync<TPayload>(string groupName, string method, TPayload payload, CancellationToken cancellationToken);
}
