using Microsoft.EntityFrameworkCore;
using Project.Domain.SomethingContext.Models;
using Project.Infrastructure.SomethingContext.EntityConfigurations;

namespace Project.Infrastructure;

public class ProjectDbContext : DbContext
{
    protected ProjectDbContext(DbContextOptions options) : base(options) { }
    public ProjectDbContext(DbContextOptions<ProjectDbContext> options) : base(options) { }

    public DbSet<SomethingAggregate> SomethingAggregates => Set<SomethingAggregate>();

    public DbSet<Architecture.Shell.EventBus.Inbox.IntegrationEventEntity> Inbox => Set<Architecture.Shell.EventBus.Inbox.IntegrationEventEntity>();

    public DbSet<Architecture.Shell.EventBus.Outbox.IntegrationEventEntity> Outbox => Set<Architecture.Shell.EventBus.Outbox.IntegrationEventEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new SomethingAggregateTypeConfiguration());
        modelBuilder.ApplyConfiguration(new EventBusContext.Inbox.EntityConfigurations.IntegrationEventEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new EventBusContext.Outbox.EntityConfigurations.IntegrationEventEntityTypeConfiguration());
    }
}
