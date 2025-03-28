namespace TekkenStats.Core.Entities;

public class OpponentInfo : PlayerInfo
{
    public OpponentInfo(PlayerInfo playerInfo, string tekkenId) : base(playerInfo.CharacterId, playerInfo.RatingBefore, playerInfo.RatingChange, playerInfo.Rounds)
    {
        TekkenId = tekkenId;
    }
    public string TekkenId { get; set; }
}