using Microsoft.EntityFrameworkCore;
using TekkenStats.Core.Entities;

namespace TekkenStats.DataAccess;

public class AppDbContext : DbContext
{
    public DbSet<Battle> Battles { get; set; }
    public DbSet<Character> Characters { get; set; }
    public DbSet<CharacterInfo> CharacterInfos { get; set; }
    public DbSet<Player> Players { get; set; }
    public DbSet<PlayerName> PlayerNames { get; set; }
    public DbSet<ProcessedMessage> ProcessedMessages { get; set; }
    
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}