using Microsoft.Extensions.Caching.Memory;
using TekkenStats.Core.Entities;
using TekkenStats.DataAccess;

namespace TekkenStats.API.Features.Shared;

public class CharacterStore
{
    private readonly IMemoryCache _cache;

    public CharacterStore(IMemoryCache cache)
    {
        _cache = cache;
    }

    public string GetCharacter(int id)
    {
        return _cache.Get<string>(id) ?? throw new Exception($"Character with id {id} not found");
    }
}