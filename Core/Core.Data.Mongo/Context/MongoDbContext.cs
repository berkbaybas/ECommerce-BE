using Core.Data.Mongo.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Core.Data.Mongo.Context;

public sealed class MongoDbContext : IMongoDbContext
{
    public MongoDbContext(IOptions<MongoSettings> options)
    {
        var settings = options.Value;
        var client = new MongoClient(settings.ConnectionString);
        this.Database = client.GetDatabase(settings.DatabaseName);
    }

    public IMongoDatabase Database { get; }

    public IMongoCollection<T> GetCollection<T>(string collectionName)
        => this.Database.GetCollection<T>(collectionName);
}
