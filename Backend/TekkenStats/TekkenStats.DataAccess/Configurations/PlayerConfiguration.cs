using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TekkenStats.Core.Entities;

namespace TekkenStats.DataAccess.Configurations;

public class PlayerConfiguration : IEntityTypeConfiguration<Player>
{
    public void Configure(EntityTypeBuilder<Player> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id).HasMaxLength(50);

        builder
            .HasMany(p => p.Names)
            .WithOne(n => n.Player)
            .HasForeignKey(n => n.PlayerId);

        builder
            .HasMany(p => p.CharacterInfos)
            .WithOne(ci => ci.Player)
            .HasForeignKey(ci => ci.PlayerId);

        builder
            .HasMany(p => p.Player1Battles)
            .WithOne(b => b.Player1)
            .HasForeignKey(b => b.Player1Id);

        builder
            .HasMany(p => p.Player2Battles)
            .WithOne(b => b.Player2)
            .HasForeignKey(b => b.Player2Id);
    }
}