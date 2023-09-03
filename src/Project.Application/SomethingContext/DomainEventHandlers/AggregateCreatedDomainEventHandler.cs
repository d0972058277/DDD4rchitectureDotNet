using Architecture.Core;
using Architecture.Shell.EventBus.Outbox;
using Project.Application.SomethingContext.IntegrationEvents;
using Project.Domain.SomethingContext.Events;

namespace Project.Application.SomethingContext.DomainEventHandlers;

public class AggregateCreatedDomainEventHandler : IDomainEventHandler<AggregateCreatedDomainEvent>
{
    private readonly IOutbox _outbox;

    public AggregateCreatedDomainEventHandler(IOutbox outbox)
    {
        _outbox = outbox;
    }

    public Task Handle(AggregateCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        var integrationEvent = new AggregateCreatedIntegrationEvent
        {
            SomethingAggregateId = notification.SomethingAggregateId
        };
        return _outbox.PublishAsync(integrationEvent, cancellationToken);
    }
}
