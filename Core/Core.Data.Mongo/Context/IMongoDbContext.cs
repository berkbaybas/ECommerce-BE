using MongoDB.Driver;

namespace Core.Data.Mongo.Context;

public interface IMongoDbContext
{
    IMongoDatabase Database { get; }

    IMongoCollection<T> GetCollection<T>(string collectionName);
}
