using Architecture.Application.CQRS;
using Project.Application.SomethingContext.Queries.GetSomething.Models;

namespace Project.Application.SomethingContext.Queries.GetSomething;

public class GetSomethingQuery : IQuery<Something>
{
    public GetSomethingQuery(Guid somethingAggregateId)
    {
        SomethingAggregateId = somethingAggregateId;
    }

    public Guid SomethingAggregateId { get; }
}
