namespace TekkenStats.Core.Entities;

public class OpponentInfo : ChallengerInfo
{
    public OpponentInfo(ChallengerInfo challengerInfo, string tekkenId) : base(challengerInfo.CharacterId, challengerInfo.RatingBefore, challengerInfo.RatingChange, challengerInfo.Rounds)
    {
        TekkenId = tekkenId;
    }
    public string TekkenId { get; set; }
}