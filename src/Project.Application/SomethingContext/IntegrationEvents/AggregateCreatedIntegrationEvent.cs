namespace Project.Application.SomethingContext.IntegrationEvents;

public class AggregateCreatedIntegrationEvent : IntegrationEventBase
{
    public AggregateCreatedIntegrationEvent(Guid somethingAggregateId)
    {
        SomethingAggregateId = somethingAggregateId;
    }

    public Guid SomethingAggregateId { get; }
}
