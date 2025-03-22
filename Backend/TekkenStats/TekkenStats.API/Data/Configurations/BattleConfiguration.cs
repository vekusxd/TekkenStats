using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TekkenStats.API.Data.Models;

namespace TekkenStats.API.Data.Configurations;

public class BattleConfiguration : IEntityTypeConfiguration<Battle>
{
    public void Configure(EntityTypeBuilder<Battle> builder)
    {
        builder.HasKey(b => b.Id);

        builder.Property(b => b.Id).HasMaxLength(50);
        builder.Property(b => b.GameVersion).HasMaxLength(50);
        builder.Property(b => b.Player1Id).HasMaxLength(50);
        builder.Property(b => b.Player2Id).HasMaxLength(50);

        builder
            .HasOne(b => b.Player1)
            .WithMany(p => p.Player1Battles)
            .HasForeignKey(b => b.Player1Id);

        builder
            .HasOne(b => b.Player2)
            .WithMany(p => p.Player2Battles)
            .HasForeignKey(b => b.Player2Id);

        builder
            .HasOne(b => b.PlayerCharacter1)
            .WithMany(c => c.Character1Battles)
            .HasForeignKey(b => b.PlayerCharacter1Id);

        builder
            .HasOne(b => b.PlayerCharacter2)
            .WithMany(c => c.Character2Battles)
            .HasForeignKey(b => b.PlayerCharacter2Id);
    }
}