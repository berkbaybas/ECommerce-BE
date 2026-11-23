using Core.Abstraction.Lifetime;
using Core.Data.Mongo.Context;
using Core.Data.Mongo.Repositories;
using Entity = Category.Domain.Entities;
namespace Category.Infrastructure.Repositories;
public interface ICategoryRepository : IMongoRepository<Entity.Category> { }
public sealed class CategoryRepository(IMongoDbContext context) : MongoRepository<Entity.Category>(context, "Categories"), ICategoryRepository, IScopedService { }
