using TekkenStats.API.Features.Shared;

namespace TekkenStats.API.Features.GetPlayerProfile;

public static class GetPlayerProfileProjectionExtensions 
{
    public static GetPlayerProfileResponse ToGetPlayerProfileResponse(
        this GetPlayerProfileProjection player,
        CharacterStore characterStore,
        string tekkenId)
    {
        return  new GetPlayerProfileResponse
        {
            TekkenId = tekkenId,
            CurrentName = player.CurrentName,
            LossCount = player.LossCount,
            WinCount = player.WinCount,
            MatchesCount = player.MatchesCount,
            Names = player.Names,
            Power = player.Power,
            Characters = player.Characters.Select(c => new CharacterResponse
            {
                CharacterId = c.CharacterId,
                CharacterName = characterStore.GetCharacter(c.CharacterId).Name ??
                                throw new NullReferenceException($"Character with id: {c.CharacterId} not found"),
                MatchesCount = c.MatchesCount,
                WinCount = c.WinCount,
                LossCount = c.LossCount,
                Rating = c.Rating,
                LastPlayed = c.LastPlayed,
                ImgURL = characterStore.GetCharacter(c.CharacterId).ImgURL ??
                         throw new Exception($"Character with id: {c.CharacterId} not found")
            }).ToList(),
        };
    }
}