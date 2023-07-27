using Architecture.Domain.MessageBus;

namespace Architecture.Application.MessageBus;

public static class EventPublisherExtensions
{
    public static Task PublishAsync<T>(this IEventPublisher eventBus, T @event, CancellationToken cancellationToken = default) where T : IIntegrationEvent
    {
        var integrationEvent = IntegrationEvent.Create(@event);
        return eventBus.PublishAsync(integrationEvent, cancellationToken);
    }
}
