using Architecture.Domain.EventBus;

namespace Architecture.Application.EventBus;

public class EventBus : IEventBus
{
    private readonly IEventPublisherFactory _eventPublisherFactory;

    public EventBus(IEventPublisherFactory eventPublisherFactory)
    {
        _eventPublisherFactory = eventPublisherFactory;
    }

    public Task PublishAsync<TIntegrationEvent>(TIntegrationEvent integrationEvent, CancellationToken cancellationToken = default) where TIntegrationEvent : IIntegrationEvent
    {
        var eventPublisher = _eventPublisherFactory.GetOutboxEventPublisher();
        return eventPublisher.PublishAsync(integrationEvent, cancellationToken);
    }
}
