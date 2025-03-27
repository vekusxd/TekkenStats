﻿using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using TekkenStats.Core.Entities;
using TekkenStats.DataAccess;

namespace TekkenStats.API.Features.SeedDb;

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
        if (_dbContext.Characters.Any()) return;
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
