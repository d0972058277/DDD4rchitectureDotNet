using Architecture.Domain.MessageBus.Inbox;

namespace Architecture.Application.MessageBus.Inbox;

public interface IIntegrationEventRepository : IRepository
{
    Task<IntegrationEventEntry> FindAsync(Guid integrationEventId, CancellationToken cancellationToken = default);
    Task AddAsync(IntegrationEventEntry integrationEventEntry, CancellationToken cancellationToken = default);
    Task SaveAsync(IntegrationEventEntry integrationEventEntry, CancellationToken cancellationToken = default);
}
