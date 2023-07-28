using Architecture.Domain.EventBus.Outbox;

namespace Architecture.Application.EventBus.Outbox;

public interface IIntegrationEventRepository : IRepository
{
    Task<IReadOnlyList<IntegrationEventEntry>> FindAsync(Guid transactionId, CancellationToken cancellationToken = default);
    Task AddAsync(IntegrationEventEntry integrationEventEntry, CancellationToken cancellationToken = default);
    Task SaveAsync(IEnumerable<IntegrationEventEntry> integrationEventEntries, CancellationToken cancellationToken = default);
}
