namespace Architecture.Shell.EventBus.Outbox;

public interface IIntegrationEventRepository : IRepository
{
    Task<IReadOnlyList<IntegrationEventEntity>> FindAsync(Guid transactionId, CancellationToken cancellationToken = default);
    Task AddAsync(IntegrationEventEntity integrationEventEntity, CancellationToken cancellationToken = default);
    Task SaveAsync(IEnumerable<IntegrationEventEntity> integrationEventEntries, CancellationToken cancellationToken = default);
}
