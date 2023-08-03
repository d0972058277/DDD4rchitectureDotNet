using Architecture.Application.EventBus;
using Architecture.Domain;
using Project.Application.SomethingContext.IntegrationEvents;
using Project.Domain.SomethingContext.Events;

namespace Project.Application.SomethingContext.Commands.CreateAggregate;

public class AggregateCreatedDomainEventHandler : IDomainEventHandler<AggregateCreatedDomainEvent>
{
    private readonly IEventOutbox _eventOutbox;

    public AggregateCreatedDomainEventHandler(IEventOutbox eventOutbox)
    {
        _eventOutbox = eventOutbox;
    }

    public Task Handle(AggregateCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        var integrationEvent = new AggregateCreatedIntegrationEvent(notification.SomethingAggregateId);
        return _eventOutbox.PublishAsync(integrationEvent, cancellationToken);
    }
}
