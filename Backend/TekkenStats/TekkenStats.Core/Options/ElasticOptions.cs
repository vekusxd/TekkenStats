namespace TekkenStats.Core.Options;

public class ElasticOptions
{
    public const string Section = "ElasticOptions";
    public required string Url { get; set; }
}