using System.Linq.Expressions;
using Core.Abstraction.Entities;
using Core.Abstraction.Paging;
using Core.Data.Mongo.Context;
using MongoDB.Driver;

namespace Core.Data.Mongo.Repositories;

public abstract class MongoRepository<T> : IMongoRepository<T>
    where T : BaseEntity
{
    private readonly IMongoCollection<T> _collection;

    protected MongoRepository(IMongoDbContext context, string collectionName)
    {
        MongoClassMapBootstrapper.EnsureBaseEntityMapRegistered();
        this._collection = context.GetCollection<T>(collectionName);
    }

    public Task<T?> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken)
        => this._collection.Find(NotDeleted().Add(predicate)).FirstOrDefaultAsync(cancellationToken)!;

    public async Task<IReadOnlyList<T>> ListAsync(Expression<Func<T, bool>>? predicate, CancellationToken cancellationToken)
    {
        var filter = predicate is null ? NotDeleted().Build() : NotDeleted().Add(predicate);
        return await this._collection.Find(filter).ToListAsync(cancellationToken);
    }

    public async Task<PagedResult<T>> ListPagedAsync(
        Expression<Func<T, bool>>? predicate,
        int page,
        int pageSize,
        string? orderByField,
        OrderDirectionType orderDirection,
        CancellationToken cancellationToken)
    {
        var filter = predicate is null ? NotDeleted().Build() : NotDeleted().Add(predicate);

        var sort = string.IsNullOrWhiteSpace(orderByField)
            ? Builders<T>.Sort.Descending(nameof(BaseEntity.CreatedAt))
            : orderDirection == OrderDirectionType.Descending
                ? Builders<T>.Sort.Descending(orderByField)
                : Builders<T>.Sort.Ascending(orderByField);

        var totalCount = await this._collection.CountDocumentsAsync(filter, cancellationToken: cancellationToken);

        var items = await this._collection.Find(filter)
            .Sort(sort)
            .Skip((page - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<T>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount,
        };
    }

    public Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken)
        => this._collection.Find(NotDeleted().Add(predicate)).AnyAsync(cancellationToken);

    public Task AddAsync(T entity, CancellationToken cancellationToken)
        => this._collection.InsertOneAsync(entity, cancellationToken: cancellationToken);

    public Task UpdateAsync(T entity, CancellationToken cancellationToken)
        => this._collection.ReplaceOneAsync(x => x.Id == entity.Id, entity, cancellationToken: cancellationToken);

    public Task DeleteAsync(string id, CancellationToken cancellationToken)
    {
        var update = Builders<T>.Update
            .Set(x => x.IsDeleted, true)
            .Set(x => x.UpdatedAt, DateTime.UtcNow);

        return this._collection.UpdateOneAsync(x => x.Id == id, update, cancellationToken: cancellationToken);
    }

    private static FilterChain NotDeleted() => new(Builders<T>.Filter.Eq(x => x.IsDeleted, false));

    private readonly struct FilterChain(FilterDefinition<T> filter)
    {
        public FilterDefinition<T> Add(Expression<Func<T, bool>> predicate)
            => Builders<T>.Filter.And(filter, Builders<T>.Filter.Where(predicate));

        public FilterDefinition<T> Build() => filter;
    }
}
