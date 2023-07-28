using Architecture.Domain.EventBus.Outbox;
using Microsoft.EntityFrameworkCore;

namespace Project.Infrastructure.EventBusContext.Outbox.Repositories;

public class IntegrationEventRepository : Architecture.Application.EventBus.Outbox.IIntegrationEventRepository
{
    private readonly ProjectDbContext _dbContext;

    public IntegrationEventRepository(ProjectDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task AddAsync(IntegrationEventEntry integrationEventEntry, CancellationToken cancellationToken = default)
    {
        _dbContext.Outbox.Add(integrationEventEntry);
        // NOTE: Outbox 是跟著 UnitOfWork 的 Transaction ，所以不需立即執行 SaveChangesAsync
        return Task.CompletedTask;
    }

    public async Task<IReadOnlyList<IntegrationEventEntry>> FindAsync(Guid transactionId, CancellationToken cancellationToken = default)
    {
        var entries = await _dbContext.Outbox.Where(e => e.TransactionId == transactionId).ToListAsync(cancellationToken);
        return entries;
    }

    public Task SaveAsync(IEnumerable<IntegrationEventEntry> integrationEventEntries, CancellationToken cancellationToken = default)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
}
