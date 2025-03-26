using TekkenStats.Application.Contracts;
using TekkenStats.Core.Entities;

namespace TekkenStats.Application.Mappers;

public static class BattleMapper
{
    public static Battle ToEntity(BattleSerialized serialized)
    {
        return new Battle
        {
            Id = serialized.Id,
            GameVersion = serialized.GameVersion,
            PlayedDateTime = serialized.PlayedDateTime,
            Player1Id = serialized.Player1Id,
            Player2Id = serialized.Player2Id,
            PlayerCharacter1Id = serialized.PlayerCharacter1Id,
            PlayerCharacter2Id = serialized.PlayerCharacter2Id,
            Player1RoundsCount = serialized.Player1RoundsCount,
            Player2RoundsCount = serialized.Player2RoundsCount,
            Player1RatingBefore = serialized.Player1RatingBefore,
            Player2RatingBefore = serialized.Player2RatingBefore,
            Player1RatingChange = serialized.Player1RatingChange,
            Player2RatingChange = serialized.Player2RatingChange,
            Winner = serialized.Winner
        };
    }

    public static BattleSerialized FromEntity(Battle entity)
    {
        return new BattleSerialized
        {
            Id = entity.Id,
            GameVersion = entity.GameVersion,
            PlayedDateTime = entity.PlayedDateTime,
            Player1Id = entity.Player1Id,
            Player2Id = entity.Player2Id,
            PlayerCharacter1Id = entity.PlayerCharacter1Id,
            PlayerCharacter2Id = entity.PlayerCharacter2Id,
            Player1RoundsCount = entity.Player1RoundsCount,
            Player2RoundsCount = entity.Player2RoundsCount,
            Player1RatingBefore = entity.Player1RatingBefore,
            Player2RatingBefore = entity.Player2RatingBefore,
            Player1RatingChange = entity.Player1RatingChange,
            Player2RatingChange = entity.Player2RatingChange,
            Winner = entity.Winner
        };
    }
    
}