using Microsoft.EntityFrameworkCore;
using Project.Domain.SomethingContext.Models;
using Project.Infrastructure.SomethingContext.EntityConfigurations;

namespace Project.Infrastructure;

public class ProjectDbContext : DbContext
{
    public ProjectDbContext(DbContextOptions options) : base(options) { }

    public DbSet<SomethingAggregate> SomethingAggregates => Set<SomethingAggregate>();
    public DbSet<SomethingValueObject> SomethingValueObjects => Set<SomethingValueObject>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new SomethingAggregateTypeConfiguration());
        modelBuilder.ApplyConfiguration(new SomethingValueObjectTypeConfiguration());
    }
}
