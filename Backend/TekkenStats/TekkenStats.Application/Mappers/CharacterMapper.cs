using TekkenStats.Application.Contracts;
using TekkenStats.Core.Entities;

namespace TekkenStats.Application.Mappers;

public static class CharacterMapper
{
    public static Character ToEntity(CharacterSerialized serialized)
    {
        return new Character
        {
            Id = serialized.Id,
            Name = serialized.Name,
            Abbreviation = serialized.Abbreviation
        };
    }

    public static CharacterSerialized FromEntity(Character entity)
    {
        return new CharacterSerialized
        {
            Id = entity.Id,
            Name = entity.Name,
            Abbreviation = entity.Abbreviation
        };
    }
}