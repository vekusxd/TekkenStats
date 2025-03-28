namespace TekkenStats.Core.Entities;

public class Name
{
    public required string PlayerName { get; init; }
    public DateTime Date { get; init; } = DateTime.UtcNow;
}