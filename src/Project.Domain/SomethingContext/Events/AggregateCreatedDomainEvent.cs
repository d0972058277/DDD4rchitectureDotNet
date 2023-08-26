using Architecture.Core;

namespace Project.Domain.SomethingContext.Events;

public class AggregateCreatedDomainEvent : IDomainEvent
{
    public AggregateCreatedDomainEvent(Guid somethingAggregateId)
    {
        SomethingAggregateId = somethingAggregateId;
    }

    public Guid SomethingAggregateId { get; }
}
