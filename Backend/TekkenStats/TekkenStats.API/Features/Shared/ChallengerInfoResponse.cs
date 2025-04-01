namespace TekkenStats.API.Features.Shared;

public class ChallengerInfoResponse
{
    public int CharacterId { get; init; }
    public required string TekkenId { get; init; }
    public required string Name { get; init; }
    public required string CharacterName { get; init; }
    public required string CharacterImgURL { get; init; }
    public int Rounds { get; init; }
    public int RatingBefore { get; init; }
    public int RatingChange { get; init; }
}