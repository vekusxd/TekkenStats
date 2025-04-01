using Microsoft.Extensions.Caching.Memory;
using TekkenStats.Core.Entities;

namespace TekkenStats.API.Features.Shared;

public class CharacterStore
{
    private readonly IMemoryCache _cache;

    public CharacterStore(IMemoryCache cache)
    {
        _cache = cache;
    }

    public Character GetCharacter(int id)
    {
        return _cache.Get<Character>(id) ?? throw new Exception($"Character with id {id} not found");
    }
}