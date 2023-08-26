using Architecture.Shell.EventBus.Inbox;
using CSharpFunctionalExtensions;

namespace Project.Infrastructure.EventBusContext.Inbox.Repositories;

public class IntegrationEventRepository : IIntegrationEventRepository
{
    private readonly ProjectDbContext _dbContext;

    public IntegrationEventRepository(ProjectDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task AddAsync(IntegrationEventEntity IntegrationEventEntity, CancellationToken cancellationToken = default)
    {
        _dbContext.Inbox.Add(IntegrationEventEntity);
        return _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<Maybe<IntegrationEventEntity>> FindAsync(Guid integrationEventId, CancellationToken cancellationToken = default)
    {
        var entry = await _dbContext.Inbox.FindAsync(new object[] { integrationEventId }, cancellationToken);
        return entry ?? Maybe<IntegrationEventEntity>.None;
    }

    public Task SaveAsync(IntegrationEventEntity IntegrationEventEntity, CancellationToken cancellationToken = default)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
}
