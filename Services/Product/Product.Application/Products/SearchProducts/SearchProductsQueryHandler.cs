using System.Linq.Expressions;
using Core.Abstraction.Paging;
using Core.CQRS.Queries;
using Product.Abstraction.Responses.Products;
using Product.Application.Products.SearchProducts;
using Product.Infrastructure.Repositories;
using Entity = Product.Domain.Entities;

namespace Product.Application.Products;

public sealed class SearchProductsQueryHandler(IProductRepository productRepository)
    : QueryHandler<SearchProductsQuery, PagedResult<ListProductDto>>
{
    public override async Task<PagedResult<ListProductDto>> Handle(SearchProductsQuery query, CancellationToken cancellationToken)
    {
        Expression<Func<Entity.Product, bool>>? predicate = null;

        if (!string.IsNullOrWhiteSpace(query.Text))
        {
            var text = query.Text.ToLowerInvariant();
            predicate = x => x.Name.ToLower().Contains(text) || x.Sku.ToLower().Contains(text);
        }

        var result = await productRepository.ListPagedAsync(
            predicate,
            query.Page,
            query.PageSize,
            query.OrderByField,
            query.OrderDirection,
            cancellationToken);

        return result.ToMapResponse();
    }
}
