namespace TekkenStats.Core.Options;

public class MongoOptions
{
    public const string Section = "MongoOptions";
    public required string ConnectionString { get; init; }
    public required string DbName { get; init; }
}