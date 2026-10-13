using Microsoft.AspNetCore.SignalR;

namespace Core.Realtime.Notifiers;

public sealed class RealtimeNotifier<THub>(IHubContext<THub> hubContext) : IRealtimeNotifier<THub>
    where THub : Hub
{
    public Task BroadcastAsync<TPayload>(string method, TPayload payload, CancellationToken cancellationToken)
        => hubContext.Clients.All.SendAsync(method, payload, cancellationToken);

    public Task NotifyGroupAsync<TPayload>(string groupName, string method, TPayload payload, CancellationToken cancellationToken)
        => hubContext.Clients.Group(groupName).SendAsync(method, payload, cancellationToken);
}
