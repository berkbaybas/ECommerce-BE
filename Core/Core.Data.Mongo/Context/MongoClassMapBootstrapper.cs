using Core.Abstraction.Entities;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace Core.Data.Mongo.Context;

/// <summary>
/// Maps <see cref="BaseEntity"/>'s protected-setter Id to the Mongo "_id" field without
/// leaking Mongo attributes into Core.Abstraction/Domain (Clean Architecture boundary).
/// </summary>
public static class MongoClassMapBootstrapper
{
    private static bool _registered;

    public static void EnsureBaseEntityMapRegistered()
    {
        if (_registered)
        {
            return;
        }

        if (!BsonClassMap.IsClassMapRegistered(typeof(BaseEntity)))
        {
            BsonClassMap.RegisterClassMap<BaseEntity>(map =>
            {
                map.AutoMap();
                map.MapIdMember(x => x.Id).SetSerializer(new StringSerializer());
                map.SetIsRootClass(true);
                map.SetIgnoreExtraElements(true);
            });
        }

        _registered = true;
    }
}
