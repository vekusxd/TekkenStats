using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage;
using TekkenStats.Core.Entities;

namespace TekkenStats.DataAccess.Configurations;

public class CharacterConfiguration : IEntityTypeConfiguration<Character>
{
    public void Configure(EntityTypeBuilder<Character> builder)
    {
        builder.HasKey(c => c.Id);

        builder
            .Property(c => c.Id)
            .ValueGeneratedNever();

        builder
            .Property(c => c.Abbreviation)
            .HasMaxLength(50);

        builder
            .Property(c => c.Name)
            .HasMaxLength(50);

        builder
            .HasMany(c => c.CharacterInfos)
            .WithOne(ci => ci.Character)
            .HasForeignKey(ci => ci.CharacterId);

        builder
            .HasMany(c => c.Character1Battles)
            .WithOne(b => b.PlayerCharacter1)
            .HasForeignKey(b => b.PlayerCharacter1Id);

        builder
            .HasMany(c => c.Character2Battles)
            .WithOne(b => b.PlayerCharacter2)
            .HasForeignKey(b => b.PlayerCharacter2Id);
    }
}