using TekkenStats.API.Features.GetPlayerProfile;
using TekkenStats.API.Features.Shared;

namespace TekkenStats.API.Features.GetMatchups;

public static class PlayerMatchStatsExtension
{
    public static MatchupsResponse ToMatchupResponse(this PlayerMatchStats playerMatchStats, CharacterStore characterStore)
    {
        return new MatchupsResponse
        {
            OpponentCharacterId = playerMatchStats.OpponentCharacterId,
            Wins = playerMatchStats.Wins,
            Losses = playerMatchStats.Losses,
            TotalMatches = playerMatchStats.TotalMatches,
            CharacterName = characterStore.GetCharacter(playerMatchStats.OpponentCharacterId).Name,
            CharacterImgURL = characterStore.GetCharacter(playerMatchStats.OpponentCharacterId).ImgURL
        };
    }
}