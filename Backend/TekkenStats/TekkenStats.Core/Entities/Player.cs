using MongoDB.Bson.Serialization.Attributes;

namespace TekkenStats.Core.Entities;

public class Player
{
    public static readonly string CollectionName = "Players";
    [BsonId] public required string TekkenId { get; init; }
    public required string CurrentName { get; set; }
    public long Power { get; set; }
    public long Rank { get; set; }
    public List<Name> Names { get; set; } = [];
    public List<Match> Matches { get; set; } = [];
}

public class Name
{
    public required string PlayerName { get; init; }
    public DateTime Date { get; init; } = DateTime.UtcNow;
}