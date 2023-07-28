using Microsoft.EntityFrameworkCore;
using Project.Domain.SomethingContext.Models;
using Project.Infrastructure.SomethingContext.EntityConfigurations;

namespace Project.Infrastructure;

public class ProjectDbContext : DbContext
{
    protected ProjectDbContext(DbContextOptions options) : base(options) { }
    public ProjectDbContext(DbContextOptions<ProjectDbContext> options) : base(options) { }

    public DbSet<SomethingAggregate> SomethingAggregates => Set<SomethingAggregate>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new SomethingAggregateTypeConfiguration());
    }
}
