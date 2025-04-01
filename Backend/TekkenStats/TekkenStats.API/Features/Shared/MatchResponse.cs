namespace TekkenStats.API.Features.Shared;

public class MatchResponse
{
    public required string BattleId { get; init; }
    public DateTime Date { get; init; }
    public long GameVersion { get; init; }
    public bool Winner { get; init; }
    public required ChallengerInfoResponse Challenger { get; init; }
    public required ChallengerInfoResponse Opponent { get; init; }
}