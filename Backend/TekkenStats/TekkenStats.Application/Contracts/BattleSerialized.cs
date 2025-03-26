namespace TekkenStats.Application.Contracts;

public class BattleSerialized
{
    public required string Id { get; set; }
    public required string GameVersion { get; set; }
    public DateTime PlayedDateTime { get; set; }
    public required string Player1Id { get; set; }
    public required string Player2Id { get; set; }
    public int PlayerCharacter1Id { get; set; }
    public int PlayerCharacter2Id { get; set; }
    public int Player1RoundsCount { get; set; }
    public int Player2RoundsCount { get; set; }
    public int Player1RatingBefore { get; set; }
    public int Player2RatingBefore { get; set; }
    public int Player1RatingChange { get; set; }
    public int Player2RatingChange { get; set; }
    public int Winner { get; set; }
}