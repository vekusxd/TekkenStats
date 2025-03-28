using System.Text.Json;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using TekkenStats.Core.Entities;
using TekkenStats.DataAccess;

namespace TekkenStats.API.Features.Seeder;

public class Seeder
{
    private readonly IWebHostEnvironment _environment;
    private readonly AppDbContext _dbContext;
    private readonly IMemoryCache _memoryCache;

    public Seeder(IWebHostEnvironment environment, AppDbContext dbContext, IMemoryCache memoryCache)
    {
        _environment = environment;
        _dbContext = dbContext;
        _memoryCache = memoryCache;
    }

    public async Task InitCache()
    {
        var cacheEntryOptions = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(DateTimeOffset.MaxValue)
            .SetPriority(CacheItemPriority.NeverRemove);
        var characters = await _dbContext.Characters.ToListAsync();
        foreach (var character in characters)
        {
            _memoryCache.Set(character.Id, character.Name, cacheEntryOptions);
        }
    }

    public async Task Migrate()
    {
        await _dbContext.Database.MigrateAsync();
    }

    public async Task SeedDb()
    {
        var filePath = Path.Combine(_environment.WebRootPath, "data", "characters.json");
        var content = await File.ReadAllTextAsync(filePath);
        var data = (JsonSerializer.Deserialize<IEnumerable<Character>>(content) ??
                    throw new InvalidOperationException("characters.json not found")).ToList();
        var existingIds = await _dbContext.Characters
            .Select(c => c.Id)
            .ToListAsync();

        var newCharacters = data
            .Where(item => !existingIds.Contains(item.Id))
            .Select(item => new Character
            {
                Id = item.Id,
                Name = item.Name,
                Abbreviation = item.Abbreviation
            })
            .ToList();

        if (newCharacters.Count != 0)
        {
            _dbContext.Characters.AddRange(newCharacters);
            await _dbContext.SaveChangesAsync();
        }
    }
}