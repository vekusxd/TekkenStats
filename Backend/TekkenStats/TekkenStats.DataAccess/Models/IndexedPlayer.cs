namespace TekkenStats.DataAccess.Models;

public class IndexedPlayer
{
    public const string IndexName = "players";
    public required string TekkenId { get; init; }
    public required string Name { get; init; }
}