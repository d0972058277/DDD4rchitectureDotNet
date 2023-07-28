using Architecture.Application.EventBus;
using Architecture.Domain;
using Project.Application.SomethingContext.IntegrationEvents;
using Project.Domain.SomethingContext.Events;

namespace Project.Application.SomethingContext.Commands.CreateAggregate;

public class AggregateCreatedDomainEventHandler : IDomainEventHandler<AggregateCreatedDomainEvent>
{
    private readonly IEventOutbox _outbox;

    public AggregateCreatedDomainEventHandler(IEventOutbox outbox)
    {
        _outbox = outbox;
    }

    public Task Handle(AggregateCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        var integrationEvent = new AggregateCreatedIntegrationEvent(notification.SomethingAggregateId);
        return _outbox.SendAsync(integrationEvent, cancellationToken);
    }
}
