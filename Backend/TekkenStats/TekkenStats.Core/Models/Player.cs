namespace TekkenStats.Core.Models;

public class Player
{
    public required string Id { get; init; }
    public int WinCount { get; set; }
    public int LossCount { get; set; }
    public ICollection<PlayerName> Names { get; set; } = [];
    public ICollection<CharacterInfo> CharacterInfos { get; set; } = [];
    public ICollection<Battle> Player1Battles { get; set; } = [];
    public ICollection<Battle> Player2Battles { get; set; } = [];
}