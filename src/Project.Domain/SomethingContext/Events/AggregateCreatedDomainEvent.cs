using Architecture.Domain;

namespace Project.Domain.SomethingContext.Events;

public class AggregateCreatedDomainEvent : IDomainEvent
{
    public AggregateCreatedDomainEvent(Guid somethingAggregateId)
    {
        SomethingAggregateId = somethingAggregateId;
    }

    public Guid SomethingAggregateId { get; }
}
