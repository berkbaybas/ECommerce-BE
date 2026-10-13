using Core.Abstraction.Exceptions;
using Core.CQRS.Queries;
using Product.Abstraction.Responses.Products;
using Product.Application.Products.FindProduct;
using Product.Core.Exceptions;
using Product.Infrastructure.Repositories;

namespace Product.Application.Products;

public sealed class FindProductQueryHandler(IProductRepository productRepository)
    : QueryHandler<FindProductQuery, ProductDto>
{
    public override async Task<ProductDto> Handle(FindProductQuery query, CancellationToken cancellationToken)
    {
        var product = await productRepository.FindAsync(x => x.Id == query.Id, cancellationToken);
        NotFoundException.ThrowIfNull(product, ProductLocalizations.Errors.Business.NotFound);

        return product!.ToMapResponse();
    }
}
