using Architecture.Core;

namespace Project.Domain.SomethingContext.Events;

public class EntityRenamedDomainEvent : IDomainEvent
{
    public EntityRenamedDomainEvent(Guid somethingAggregateId)
    {
        SomethingAggregateId = somethingAggregateId;
    }

    public Guid SomethingAggregateId { get; }
}
