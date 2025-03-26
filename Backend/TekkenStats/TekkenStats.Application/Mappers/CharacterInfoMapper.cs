using TekkenStats.Application.Contracts;
using TekkenStats.Core.Entities;

namespace TekkenStats.Application.Mappers;

public static class CharacterInfoMapper
{
    public static CharacterInfo ToEntity(CharacterInfoSerialized serialized)
    {
        return new CharacterInfo
        {
            Id = serialized.Id,
            CharacterId = serialized.CharacterId,
            PlayerId = serialized.PlayerId,
            LastPlayed = serialized.LastPlayed,
            Rating = serialized.Rating,
            WinCount = serialized.WinCount,
            LossCount = serialized.LossCount
        };
    }

    public static CharacterInfoSerialized FromEntity(CharacterInfo entity)
    {
        return new CharacterInfoSerialized
        {
            Id = entity.Id,
            CharacterId = entity.CharacterId,
            PlayerId = entity.PlayerId,
            LastPlayed = entity.LastPlayed,
            LossCount = entity.LossCount,
            WinCount = entity.WinCount,
            Rating = entity.Rating
        };
    }
}