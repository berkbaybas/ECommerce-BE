using Core.Abstraction.Exceptions;
using Core.CQRS.Commands;
using MediatR;
using Product.Core.Exceptions;
using Product.Infrastructure.Repositories;

namespace Product.Application.Products;

public sealed class UpdateProductDetailsCommandHandler(IProductRepository productRepository)
    : CommandHandler<UpdateProductDetailsCommand, Unit>
{
    public override async Task<Unit> Handle(UpdateProductDetailsCommand command, CancellationToken cancellationToken)
    {
        var product = await productRepository.FindAsync(x => x.Id == command.Id, cancellationToken);
        NotFoundException.ThrowIfNull(product, ProductLocalizations.Errors.Business.NotFound);

        product!.UpdateDetails(command.Name, command.Description);
        await productRepository.UpdateAsync(product, cancellationToken);

        return Unit.Value;
    }
}
