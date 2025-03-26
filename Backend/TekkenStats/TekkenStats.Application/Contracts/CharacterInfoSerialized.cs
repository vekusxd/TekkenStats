namespace TekkenStats.Application.Contracts;

public class CharacterInfoSerialized
{
    public required  Guid Id { get; set; }
    public int CharacterId { get; set; }
    public required string PlayerId { get; set; }
    public int WinCount { get; set; }
    public int LossCount { get; set; }
    public DateOnly LastPlayed { get; set; }
    public int Rating { get; set; }
}