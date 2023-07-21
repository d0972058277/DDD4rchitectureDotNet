using Project.Application.SomethingContext.Repositories;
using Project.Domain.SomethingContext.Models;

namespace Project.Infrastructure.SomethingContext.Repositories;

public class SomethingRepository : ISomethingRepository
{
    private readonly ProjectDbContext _dbContext;

    public SomethingRepository(ProjectDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task AddAsync(SomethingAggregate aggregate, CancellationToken cancellationToken)
    {
        _dbContext.SomethingAggregates.Add(aggregate);
        return Task.CompletedTask;
    }

    public async Task<SomethingAggregate> FindAsync(Guid id, CancellationToken cancellationToken)
    {
        return (await _dbContext.SomethingAggregates.FindAsync(new object[] { id }, cancellationToken: cancellationToken))!;
    }

    public Task SaveAsync(SomethingAggregate aggregate, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
