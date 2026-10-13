using Core.Abstraction.Lifetime;
using Core.Data.Mongo.Context;
using Core.Data.Mongo.Repositories;
using Entity = Product.Domain.Entities;

namespace Product.Infrastructure.Repositories;

public sealed class ProductRepository(IMongoDbContext context)
    : MongoRepository<Entity.Product>(context, CollectionName), IProductRepository, IScopedService
{
    private const string CollectionName = "Products";
}
