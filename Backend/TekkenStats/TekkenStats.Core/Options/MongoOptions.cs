namespace TekkenStats.Core.Options;

public class MongoOptions
{
    public const string Section = "MongoOptions";
    public required string ConnectionString { get; set; }
    public required string DbName { get; set; }
}