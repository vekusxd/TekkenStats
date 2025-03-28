namespace TekkenStats.Core.Entities;

public class Match
{
    public required string BattleId { get; set; }
    public DateTime Date { get; set; }
    public long GameVersion { get; set; }
    public bool Winner { get; set; }
    public required ChallengerInfo Challenger { get; set; }
    public required OpponentInfo Opponent { get; set; }
}