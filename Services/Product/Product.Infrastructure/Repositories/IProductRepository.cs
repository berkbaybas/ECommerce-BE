using Core.Data.Mongo.Repositories;
using Entity = Product.Domain.Entities;

namespace Product.Infrastructure.Repositories;

public interface IProductRepository : IMongoRepository<Entity.Product>
{
}
