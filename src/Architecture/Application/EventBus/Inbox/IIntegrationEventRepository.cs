using Architecture.Domain.EventBus.Inbox;
using CSharpFunctionalExtensions;

namespace Architecture.Application.EventBus.Inbox;

public interface IIntegrationEventRepository : IRepository
{
    Task<Maybe<IntegrationEventEntry>> FindAsync(Guid integrationEventId, CancellationToken cancellationToken = default);
    Task AddAsync(IntegrationEventEntry integrationEventEntry, CancellationToken cancellationToken = default);
    Task SaveAsync(IntegrationEventEntry integrationEventEntry, CancellationToken cancellationToken = default);
}
