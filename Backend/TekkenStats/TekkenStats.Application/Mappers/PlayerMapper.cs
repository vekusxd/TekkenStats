using TekkenStats.Application.Contracts;
using TekkenStats.Core.Entities;

namespace TekkenStats.Application.Mappers;

public static class PlayerMapper
{
    public static Player ToEntity(PlayerSerialized serialized)
    {
        return new Player
        {
            Id = serialized.Id,
            WinCount = serialized.WinCount,
            LossCount = serialized.LossCount,
        };
    }

    public static PlayerSerialized FromEntity(Player entity)
    {
        return new PlayerSerialized
        {
            Id = entity.Id,
            WinCount = entity.WinCount,
            LossCount = entity.LossCount
        };
    }
}