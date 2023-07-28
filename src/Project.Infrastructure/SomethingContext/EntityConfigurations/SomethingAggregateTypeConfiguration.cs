using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Project.Domain.SomethingContext.Models;

namespace Project.Infrastructure.SomethingContext.EntityConfigurations;

public class SomethingAggregateTypeConfiguration : IEntityTypeConfiguration<SomethingAggregate>
{
    public void Configure(EntityTypeBuilder<SomethingAggregate> builder)
    {
        builder.ToTable("SomethingAggregate");

        builder.HasKey(e => e.Id);
        builder.Ignore(e => e.DomainEvents);

        builder.OwnsOne(e => e.Entity, entity =>
        {
            entity.Property(e => e.Id).HasColumnName("EntityId");
            entity.Property(e => e.Name).HasColumnName("EntityName");
        });

        builder.OwnsMany(e => e.ValueObjects, vo =>
        {
            vo.ToTable("SomethingAggregate_ValueObject");

            vo.Property<long>("_id")
                .ValueGeneratedOnAdd()
                .IsRequired();
            vo.HasKey("_id");

            vo.Property(e => e.String);
            vo.Property(e => e.Number);
            vo.Property(e => e.Boolean);
            vo.Property(e => e.DateTime);
        });
    }
}
