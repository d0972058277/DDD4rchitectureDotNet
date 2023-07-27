using Architecture.Domain.MessageBus.Outbox;

namespace Architecture.Application.MessageBus.Outbox;

public interface IIntegrationEventRepository : IRepository
{
    Task<IReadOnlyList<IntegrationEventEntry>> FindAsync(IEnumerable<Guid> transactionIds, CancellationToken cancellationToken = default);
    Task AddAsync(IntegrationEventEntry integrationEventEntry, CancellationToken cancellationToken = default);
    Task SaveAsync(IEnumerable<IntegrationEventEntry> integrationEventEntries, CancellationToken cancellationToken = default);
}