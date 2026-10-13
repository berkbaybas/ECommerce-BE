using Core.Abstraction.Paging;

namespace Core.CQRS.Queries;

public abstract class PagedQuery<TItem>(
    int? page,
    int? pageSize,
    string? orderByField,
    OrderDirectionType? orderDirection) : IQuery<PagedResult<TItem>>
{
    public int Page { get; } = page is > 0 ? page.Value : 1;

    public int PageSize { get; } = pageSize is > 0 ? pageSize.Value : 20;

    public string? OrderByField { get; } = orderByField;

    public OrderDirectionType OrderDirection { get; } = orderDirection ?? OrderDirectionType.Ascending;
}
