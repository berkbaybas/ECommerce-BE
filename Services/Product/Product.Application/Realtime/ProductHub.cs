using Microsoft.AspNetCore.SignalR;

namespace Product.Application.Realtime;

/// <summary>
/// Broadcast-only hub for stock/price changes. Clients join a product's group to receive
/// updates scoped to that product, or stay ungrouped to receive the full-catalog broadcast.
/// </summary>
public sealed class ProductHub : Hub
{
    public Task SubscribeToProduct(string productId) => this.Groups.AddToGroupAsync(this.Context.ConnectionId, productId);

    public Task UnsubscribeFromProduct(string productId) => this.Groups.RemoveFromGroupAsync(this.Context.ConnectionId, productId);
}
