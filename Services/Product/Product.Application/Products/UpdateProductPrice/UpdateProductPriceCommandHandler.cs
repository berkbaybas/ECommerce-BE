using Core.Abstraction.Exceptions;
using Core.CQRS.Commands;
using Core.Realtime.Notifiers;
using Product.Abstraction.Responses.Products;
using Product.Application.Products.UpdateProductPrice;
using Product.Application.Realtime;
using Product.Core.Exceptions;
using Product.Infrastructure.Repositories;

namespace Product.Application.Products;

public sealed class UpdateProductPriceCommandHandler(
    IProductRepository productRepository,
    IRealtimeNotifier<ProductHub> realtimeNotifier
) : CommandHandler<UpdateProductPriceCommand, ProductDto>
{
    public override async Task<ProductDto> Handle(UpdateProductPriceCommand command, CancellationToken cancellationToken)
    {
        var product = await productRepository.FindAsync(x => x.Id == command.Id, cancellationToken);
        NotFoundException.ThrowIfNull(product, ProductLocalizations.Errors.Business.NotFound);

        product!.UpdatePrice(command.Price);
        await productRepository.UpdateAsync(product, cancellationToken);

        var payload = new ProductPriceChangedPayload { ProductId = product.Id, Price = product.Price };
        await realtimeNotifier.BroadcastAsync(RealtimeMethods.PriceUpdated, payload, cancellationToken);
        await realtimeNotifier.NotifyGroupAsync(product.Id, RealtimeMethods.PriceUpdated, payload, cancellationToken);

        return product.ToMapResponse();
    }
}
