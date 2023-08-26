using CSharpFunctionalExtensions;

namespace Architecture.Shell.EventBus.Inbox;

public interface IIntegrationEventRepository : IRepository
{
    Task<Maybe<IntegrationEventEntity>> FindAsync(Guid integrationEventId, CancellationToken cancellationToken = default);
    Task AddAsync(IntegrationEventEntity integrationEventEntity, CancellationToken cancellationToken = default);
    Task SaveAsync(IntegrationEventEntity integrationEventEntity, CancellationToken cancellationToken = default);
}
