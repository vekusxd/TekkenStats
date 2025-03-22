using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TekkenStats.API.Data.Models;

namespace TekkenStats.API.Data.Configurations;

public class PlayerNameConfiguration : IEntityTypeConfiguration<PlayerName>
{
    public void Configure(EntityTypeBuilder<PlayerName> builder)
    {
        builder.HasKey(n => n.Id);

        builder
            .Property(n => n.Name)
            .IsRequired()
            .HasMaxLength(255);

        builder
            .HasOne(n => n.Player)
            .WithMany(p => p.Names)
            .HasForeignKey(n => n.PlayerId);
    }
}