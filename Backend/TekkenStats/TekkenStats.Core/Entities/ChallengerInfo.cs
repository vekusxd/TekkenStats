namespace TekkenStats.Core.Entities;

public class ChallengerInfo
{
    public required int CharacterId { get; set; }
    public required int Rounds { get; set; }
    public required int RatingBefore { get; set; }
    public required int RatingChange { get; set; }    
    public required string TekkenId { get; init;  }
    public required string Name { get; init; }
}