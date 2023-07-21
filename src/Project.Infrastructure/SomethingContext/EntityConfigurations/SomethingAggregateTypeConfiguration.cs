using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Project.Domain.SomethingContext.Models;

namespace Project.Infrastructure.SomethingContext.EntityConfigurations;

public class SomethingAggregateTypeConfiguration : IEntityTypeConfiguration<SomethingAggregate>
{
    public void Configure(EntityTypeBuilder<SomethingAggregate> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Ignore(e => e.DomainEvents);

        builder.OwnsOne(e => e.Entity, entity =>
        {
            entity.Property(e => e.Id).HasColumnName("EntityId");
            entity.Property(e => e.Name).HasColumnName("EntityName");
        });

        builder.HasMany(e => e.ValueObjects)
            .WithOne()
            .HasForeignKey("SomethingAggregateId")
            .OnDelete(DeleteBehavior.Cascade)
            .Metadata.GetNavigation(false)!.SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}
