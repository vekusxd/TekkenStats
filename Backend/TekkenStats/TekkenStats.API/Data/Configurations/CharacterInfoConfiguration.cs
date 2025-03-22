using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TekkenStats.API.Data.Models;

namespace TekkenStats.API.Data.Configurations;

public class CharacterInfoConfiguration : IEntityTypeConfiguration<CharacterInfo>
{
    public void Configure(EntityTypeBuilder<CharacterInfo> builder)
    {
        builder.HasKey(ci => ci.Id);
        
        builder
            .HasOne(ci => ci.Character)
            .WithMany(c => c.CharacterInfos)
            .HasForeignKey(ci => ci.CharacterId);

        builder
            .HasOne(ci => ci.Player)
            .WithMany(p => p.CharacterInfos)
            .HasForeignKey(ci => ci.PlayerId);
    }
}