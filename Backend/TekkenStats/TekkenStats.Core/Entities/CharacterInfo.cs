namespace TekkenStats.Core.Entities;

public class CharacterInfo
{
    public required int CharacterId { get; init; }
    public int MatchesCount { get; set; }
    public int WinCount { get; set; }
    public int LossCount { get; set; }
    public int Rating { get; set; }
    public DateTime LastPlayed { get; set; }
}