using Core.Data.Mongo.Context;
using Core.Data.Mongo.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Data.Mongo;

public static class CoreDataMongoServiceCollectionExtensions
{
    public static IServiceCollection AddCoreDataMongo(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<MongoSettings>(configuration.GetSection(MongoSettings.SectionName));
        services.AddSingleton<IMongoDbContext, MongoDbContext>();

        return services;
    }
}
