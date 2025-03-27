using TekkenStats.Core.Models;

namespace TekkenStats.API.Features.GetNames;

public static class GetNamesMappingExtension
{
    public static GetNamesResponse ToResponse(this PlayerName playerName)
    {
        return new GetNamesResponse
        {
            PlayerId = playerName.PlayerId,
            Name = playerName.Name
        };
    }
}