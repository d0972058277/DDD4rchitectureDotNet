using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Project.Infrastructure.EventBusContext.Inbox.EntityConfigurations;

public class IntegrationEventEntityTypeConfiguration : IEntityTypeConfiguration<Architecture.Shell.EventBus.Inbox.IntegrationEventEntity>
{
    public void Configure(EntityTypeBuilder<Architecture.Shell.EventBus.Inbox.IntegrationEventEntity> builder)
    {
        builder.ToTable("Inbox");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.CreationTimestamp);

        builder.Property(e => e.TypeName);

        builder.Property(e => e.Content);

        builder.Property(e => e.State)
            .HasConversion<string>()
            .HasMaxLength(32)
            .HasDefaultValue(Architecture.Shell.EventBus.Inbox.State.Received)
            .IsRequired();
    }
}
