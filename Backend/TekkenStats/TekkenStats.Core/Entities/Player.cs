using MongoDB.Bson.Serialization.Attributes;

namespace TekkenStats.Core.Entities;

public class Player
{
    public static readonly string CollectionName = "Players";
    [BsonId]
    public required string TekkenId { get; set; }
    public long Power { get; set; }
    public long Rank { get; set; }
    public List<string> Names { get; set; } = [];
    public List<Match> Matches { get; set; } = [];
}