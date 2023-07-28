using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Project.Infrastructure.EventBusContext.Inbox.EntityConfigurations;

public class IntegrationEventEntryTypeConfiguration : IEntityTypeConfiguration<Architecture.Domain.EventBus.Inbox.IntegrationEventEntry>
{
    public void Configure(EntityTypeBuilder<Architecture.Domain.EventBus.Inbox.IntegrationEventEntry> builder)
    {
        builder.ToTable("Inbox");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.CreationTimestamp);

        builder.Property(e => e.TypeName);

        builder.Property(e => e.Content);

        builder.Property(e => e.State)
            .HasConversion<string>()
            .HasMaxLength(32)
            .HasDefaultValue(Architecture.Domain.EventBus.Inbox.State.Received)
            .IsRequired();
    }
}
