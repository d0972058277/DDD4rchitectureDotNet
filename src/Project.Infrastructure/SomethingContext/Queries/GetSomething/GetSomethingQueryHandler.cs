using Architecture.Application.CQRS;
using Project.Application.SomethingContext.Queries.GetSomething;
using Project.Application.SomethingContext.Queries.GetSomething.Models;

namespace Project.Infrastructure.SomethingContext.Queries.GetSomething;

public class GetSomethingQueryHandler : IQueryHandler<GetSomethingQuery, Something>
{
    private readonly ReadOnlyProjectDbContext _dbContext;

    public GetSomethingQueryHandler(ReadOnlyProjectDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Something> Handle(GetSomethingQuery request, CancellationToken cancellationToken)
    {
        var aggregate = await _dbContext.SomethingAggregates.FindAsync(new object[] { request.SomethingAggregateId }, cancellationToken);
        var vos = aggregate!.ValueObjects.Select(obj => SomethingVO.Create(obj)).ToList();
        var something = Something.Create(aggregate.Id, aggregate.Entity.Name, vos);
        return something;
    }
}
