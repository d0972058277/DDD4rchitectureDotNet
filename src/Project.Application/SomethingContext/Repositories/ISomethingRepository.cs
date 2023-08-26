using Architecture.Shell;
using Project.Domain.SomethingContext.Models;

namespace Project.Application.SomethingContext.Repositories;

public interface ISomethingRepository : IRepository
{
    Task AddAsync(SomethingAggregate aggregate, CancellationToken cancellationToken);
    Task<SomethingAggregate> FindAsync(Guid id, CancellationToken cancellationToken);
    Task SaveAsync(SomethingAggregate aggregate, CancellationToken cancellationToken);
}
