using Core.Abstraction.Exceptions;
using Core.CQRS.Commands;
using Core.Realtime.Notifiers;
using Product.Abstraction.Responses.Products;
using Product.Application.Products.UpdateProductStock;
using Product.Application.Realtime;
using Product.Core.Exceptions;
using Product.Infrastructure.Repositories;

namespace Product.Application.Products;

public sealed class UpdateProductStockCommandHandler(
    IProductRepository productRepository,
    IRealtimeNotifier<ProductHub> realtimeNotifier
) : CommandHandler<UpdateProductStockCommand, ProductDto>
{
    public override async Task<ProductDto> Handle(UpdateProductStockCommand command, CancellationToken cancellationToken)
    {
        var product = await productRepository.FindAsync(x => x.Id == command.Id, cancellationToken);
        NotFoundException.ThrowIfNull(product, ProductLocalizations.Errors.Business.NotFound);

        product!.UpdateStock(command.Stock);
        await productRepository.UpdateAsync(product, cancellationToken);

        var payload = new ProductStockChangedPayload { ProductId = product.Id, Stock = product.Stock };
        await realtimeNotifier.BroadcastAsync(RealtimeMethods.StockUpdated, payload, cancellationToken);
        await realtimeNotifier.NotifyGroupAsync(product.Id, RealtimeMethods.StockUpdated, payload, cancellationToken);

        return product.ToMapResponse();
    }
}
