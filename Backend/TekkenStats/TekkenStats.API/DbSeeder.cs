using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using TekkenStats.Core.Models;
using TekkenStats.DataAccess;

namespace TekkenStats.API;

public class DbSeeder
{
    private readonly IWebHostEnvironment _environment;
    private readonly AppDbContext _dbContext;

    public DbSeeder(IWebHostEnvironment environment, AppDbContext dbContext)
    {
        _environment = environment;
        _dbContext = dbContext;
    }

    public async Task SeedDb()
    {
        var filePath = Path.Combine(_environment.WebRootPath, "data", "characters.json");
        var content = await File.ReadAllTextAsync(filePath);
        var data = JsonSerializer.Deserialize<IEnumerable<Character>>(content);
        foreach (var item in data!)
        {
            var character = await _dbContext.Characters.FirstOrDefaultAsync(c => c.Id == item.Id);
            if (character != null) continue;

            character = new Character
            {
                Id = item.Id,
                Name = item.Name,
                Abbreviation = item.Abbreviation
            };

            _dbContext.Characters.Add(character);
            await _dbContext.SaveChangesAsync();
        }
    }
}
