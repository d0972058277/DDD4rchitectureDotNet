using Architecture.Domain.EventBus.Inbox;

namespace Architecture.Application.EventBus.Inbox;

public interface IIntegrationEventRepository : IRepository
{
    // TODO: 改成 Maybe<IntegrationEventEntry>
    Task<IntegrationEventEntry> FindAsync(Guid integrationEventId, CancellationToken cancellationToken = default);
    Task AddAsync(IntegrationEventEntry integrationEventEntry, CancellationToken cancellationToken = default);
    Task SaveAsync(IntegrationEventEntry integrationEventEntry, CancellationToken cancellationToken = default);
}
