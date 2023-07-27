using Architecture.Domain.MessageBus.Inbox;
using CSharpFunctionalExtensions;

namespace Architecture.Application.MessageBus.Inbox;

public interface IIntegrationEventRepository : IRepository
{
    Task<IReadOnlyList<IntegrationEventEntry>> FindAsync(IEnumerable<Guid> integrationEventIds, CancellationToken cancellationToken = default);
    Task AddAsync(IntegrationEventEntry integrationEventEntry, CancellationToken cancellationToken = default);
    Task SaveAsync(IEnumerable<IntegrationEventEntry> integrationEventEntries, CancellationToken cancellationToken = default);
}
