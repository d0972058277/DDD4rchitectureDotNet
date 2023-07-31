using Architecture.Application.EventBus;
using Architecture.Domain;
using Project.Application.SomethingContext.IntegrationEvents;
using Project.Domain.SomethingContext.Events;

namespace Project.Application.SomethingContext.Commands.CreateAggregate;

public class AggregateCreatedDomainEventHandler : IDomainEventHandler<AggregateCreatedDomainEvent>
{
    private readonly IEventPublisher _eventPublisher;

    public AggregateCreatedDomainEventHandler(IEventPublisher eventPublisher)
    {
        _eventPublisher = eventPublisher;
    }

    public Task Handle(AggregateCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        var integrationEvent = new AggregateCreatedIntegrationEvent(notification.SomethingAggregateId);
        return _eventPublisher.PublishAsync(integrationEvent, cancellationToken);
    }
}
