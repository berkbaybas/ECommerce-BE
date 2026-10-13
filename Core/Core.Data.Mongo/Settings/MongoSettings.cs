namespace Core.Data.Mongo.Settings;

public sealed class MongoSettings
{
    public const string SectionName = "MongoSettings";

    public string ConnectionString { get; init; } = string.Empty;

    public string DatabaseName { get; init; } = string.Empty;
}
