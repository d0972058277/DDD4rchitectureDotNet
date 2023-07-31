using Architecture.Application.EventBus;
using Architecture.Domain;
using Project.Application.SomethingContext.IntegrationEvents;
using Project.Domain.SomethingContext.Events;

namespace Project.Application.SomethingContext.Commands.CreateAggregate;

public class AggregateCreatedDomainEventHandler : IDomainEventHandler<AggregateCreatedDomainEvent>
{
    private readonly IEventBus _eventBus;

    public AggregateCreatedDomainEventHandler(IEventBus eventBus)
    {
        _eventBus = eventBus;
    }

    public Task Handle(AggregateCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        var integrationEvent = new AggregateCreatedIntegrationEvent(notification.SomethingAggregateId);
        return _eventBus.PublishAsync(integrationEvent, cancellationToken);
    }
}
