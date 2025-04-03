using TekkenStats.API.Features.GetMatchHistory;
using TekkenStats.API.Features.Shared;

namespace TekkenStats.API.Features.GetHeadToHead;

public static class HeadToHeadMatchesProjectionExtension
{
    public static GetHeadToHeadResponse ToHeadToHeadResponse(
        this HeadToHeadMatchesProjection result,
        CharacterStore characterStore)
    {
        return  new GetHeadToHeadResponse
        {
            Matches = result.Matches.Select(m => new MatchResponse
            {
                BattleId = m.BattleId,
                Date = m.Date,
                Winner = m.Winner,
                GameVersion = m.GameVersion,
                Challenger = new ChallengerInfoResponse
                {
                    TekkenId = m.Challenger.TekkenId,
                    Name = m.Challenger.Name,
                    CharacterId = m.Challenger.CharacterId,
                    CharacterName = characterStore.GetCharacter(m.Challenger.CharacterId).Name,
                    Rounds = m.Challenger.Rounds,
                    RatingBefore = m.Challenger.RatingBefore,
                    RatingChange = m.Challenger.RatingChange,
                    CharacterImgURL = characterStore.GetCharacter(m.Challenger.CharacterId).ImgURL
                },
                Opponent = new ChallengerInfoResponse
                {
                    TekkenId = m.Opponent.TekkenId,
                    Name = m.Opponent.Name,
                    CharacterId = m.Opponent.CharacterId,
                    CharacterName = characterStore.GetCharacter(m.Opponent.CharacterId).Name,
                    Rounds = m.Opponent.Rounds,
                    RatingBefore = m.Opponent.RatingBefore,
                    RatingChange = m.Opponent.RatingChange,
                    CharacterImgURL = characterStore.GetCharacter(m.Opponent.CharacterId).ImgURL,
                }
            }).ToList(),
            TotalMatches = result.TotalMatches,
            WinCount = result.WinCount,
            LossCount = result.LossCount
        };
    }
}