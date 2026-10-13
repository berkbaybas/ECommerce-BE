using Core.Abstraction.Paging;
using Core.CQRS.Queries;
using Product.Abstraction.Responses.Products;

namespace Product.Application.Products;

public sealed class SearchProductsQuery(
    string? text,
    int? page,
    int? pageSize,
    string? orderByField,
    OrderDirectionType? orderDirection)
    : PagedQuery<ListProductDto>(page, pageSize, orderByField, orderDirection)
{
    public string? Text { get; } = text;
}
