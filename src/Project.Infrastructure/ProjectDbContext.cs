using Microsoft.EntityFrameworkCore;
using Project.Domain.SomethingContext.Models;
using Project.Infrastructure.SomethingContext.EntityConfigurations;

namespace Project.Infrastructure;

public class ProjectDbContext : DbContext
{
    protected ProjectDbContext(DbContextOptions options) : base(options) { }
    public ProjectDbContext(DbContextOptions<ProjectDbContext> options) : base(options) { }

    public DbSet<SomethingAggregate> SomethingAggregates => Set<SomethingAggregate>();

    public DbSet<Architecture.Domain.EventBus.Inbox.IntegrationEventEntry> Inbox => Set<Architecture.Domain.EventBus.Inbox.IntegrationEventEntry>();

    public DbSet<Architecture.Domain.EventBus.Outbox.IntegrationEventEntry> Outbox => Set<Architecture.Domain.EventBus.Outbox.IntegrationEventEntry>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new SomethingAggregateTypeConfiguration());
        modelBuilder.ApplyConfiguration(new EventBusContext.Inbox.EntityConfigurations.IntegrationEventEntryTypeConfiguration());
        modelBuilder.ApplyConfiguration(new EventBusContext.Outbox.EntityConfigurations.IntegrationEventEntryTypeConfiguration());
    }
}
