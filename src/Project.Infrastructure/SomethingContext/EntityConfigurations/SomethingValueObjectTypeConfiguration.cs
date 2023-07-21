using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Project.Domain.SomethingContext.Models;

namespace Project.Infrastructure.SomethingContext.EntityConfigurations;

public class SomethingValueObjectTypeConfiguration : IEntityTypeConfiguration<SomethingValueObject>
{
    public void Configure(EntityTypeBuilder<SomethingValueObject> builder)
    {
        builder.Property<long>("_id")
            .ValueGeneratedOnAdd()
            .IsRequired();
        builder.HasKey("_id");

        builder.Property(e => e.String);
        builder.Property(e => e.Number);
        builder.Property(e => e.Boolean);
        builder.Property(e => e.DateTime);
    }
}
