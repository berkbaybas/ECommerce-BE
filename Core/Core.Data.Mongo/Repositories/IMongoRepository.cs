using System.Linq.Expressions;
using Core.Abstraction.Entities;
using Core.Abstraction.Paging;

namespace Core.Data.Mongo.Repositories;

public interface IMongoRepository<T>
    where T : BaseEntity
{
    Task<T?> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken);

    Task<IReadOnlyList<T>> ListAsync(Expression<Func<T, bool>>? predicate, CancellationToken cancellationToken);

    Task<PagedResult<T>> ListPagedAsync(
        Expression<Func<T, bool>>? predicate,
        int page,
        int pageSize,
        string? orderByField,
        OrderDirectionType orderDirection,
        CancellationToken cancellationToken);

    Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken);

    Task AddAsync(T entity, CancellationToken cancellationToken);

    Task UpdateAsync(T entity, CancellationToken cancellationToken);

    Task DeleteAsync(string id, CancellationToken cancellationToken);
}
