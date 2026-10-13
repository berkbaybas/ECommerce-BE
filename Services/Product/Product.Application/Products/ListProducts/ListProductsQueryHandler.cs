using Core.CQRS.Queries;
using Product.Abstraction.Responses.Products;
using Product.Application.Products.ListProducts;
using Product.Infrastructure.Repositories;

namespace Product.Application.Products;

public sealed class ListProductsQueryHandler(IProductRepository productRepository)
    : QueryHandler<ListProductsQuery, IList<ListProductDto>>
{
    public override async Task<IList<ListProductDto>> Handle(ListProductsQuery query, CancellationToken cancellationToken)
    {
        var products = await productRepository.ListAsync(predicate: null, cancellationToken);

        return products.Select(x => x.ToMapResponse()).ToList();
    }
}
