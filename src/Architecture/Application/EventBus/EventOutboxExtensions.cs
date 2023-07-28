using Architecture.Domain.EventBus;

namespace Architecture.Application.EventBus;

public static class EventOutboxExtensions
{
    public static Task PublishAsync<T>(this IEventOutbox eventBus, T @event, CancellationToken cancellationToken = default) where T : IIntegrationEvent
    {
        var integrationEvent = IntegrationEvent.Create(@event);
        return eventBus.SendAsync(integrationEvent, cancellationToken);
    }
}
