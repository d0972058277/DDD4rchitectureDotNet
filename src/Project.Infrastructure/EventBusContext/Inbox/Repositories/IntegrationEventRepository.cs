using Architecture.Domain.EventBus.Inbox;

namespace Project.Infrastructure.EventBusContext.Inbox.Repositories;

public class IntegrationEventRepository : Architecture.Application.EventBus.Inbox.IIntegrationEventRepository
{
    private readonly ProjectDbContext _dbContext;

    public IntegrationEventRepository(ProjectDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task AddAsync(IntegrationEventEntry integrationEventEntry, CancellationToken cancellationToken = default)
    {
        _dbContext.Inbox.Add(integrationEventEntry);
        return _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<IntegrationEventEntry> FindAsync(Guid integrationEventId, CancellationToken cancellationToken = default)
    {
        var entry = await _dbContext.Inbox.FindAsync(new object[] { integrationEventId }, cancellationToken);
        return entry!;
    }

    public Task SaveAsync(IntegrationEventEntry integrationEventEntry, CancellationToken cancellationToken = default)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
}
