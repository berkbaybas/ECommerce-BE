using Core.Abstraction.Exceptions;
using Core.CQRS.Commands;
using MediatR;
using Product.Core.Exceptions;
using Product.Infrastructure.Repositories;

namespace Product.Application.Products;

public sealed class DeleteProductCommandHandler(IProductRepository productRepository)
    : CommandHandler<DeleteProductCommand, Unit>
{
    public override async Task<Unit> Handle(DeleteProductCommand command, CancellationToken cancellationToken)
    {
        var exists = await productRepository.ExistsAsync(x => x.Id == command.Id, cancellationToken);
        NotFoundException.ThrowIf(!exists, ProductLocalizations.Errors.Business.NotFound);

        await productRepository.DeleteAsync(command.Id, cancellationToken);

        return Unit.Value;
    }
}
