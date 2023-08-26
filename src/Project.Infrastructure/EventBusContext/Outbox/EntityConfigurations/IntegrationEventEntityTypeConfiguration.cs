using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Project.Infrastructure.EventBusContext.Outbox.EntityConfigurations;

public class IntegrationEventEntityTypeConfiguration : IEntityTypeConfiguration<Architecture.Shell.EventBus.Outbox.IntegrationEventEntity>
{
    public void Configure(EntityTypeBuilder<Architecture.Shell.EventBus.Outbox.IntegrationEventEntity> builder)
    {
        builder.ToTable("Outbox");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.CreationTimestamp);

        builder.Property(e => e.TypeName);

        builder.Property(e => e.Content);

        builder.Property(e => e.State)
            .HasConversion<string>()
            .HasMaxLength(32)
            .HasDefaultValue(Architecture.Shell.EventBus.Outbox.State.Raised)
            .IsRequired();

        builder.Property(e => e.TransactionId);

        builder.HasIndex(e => e.TransactionId);
    }
}
