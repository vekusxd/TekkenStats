namespace TekkenStats.Core.Models;

public class CharacterInfo
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public int CharacterId { get; set; }
    public Character Character { get; init; } = null!;
    public required string PlayerId { get; set; }
    public Player Player { get; set; } = null!;
    public int WinCount { get; set; }
    public int LossCount { get; set; }
    public DateOnly LastPlayed { get; set; }
    public int Rating { get; set; }
}