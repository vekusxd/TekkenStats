using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TekkenStats.Core.Entities;

namespace TekkenStats.DataAccess.Configurations;

public class ProcessedMessageConfiguration : IEntityTypeConfiguration<ProcessedMessage>
{
    public void Configure(EntityTypeBuilder<ProcessedMessage> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasIndex(x => x.MessageId).IsUnique();
    }
}