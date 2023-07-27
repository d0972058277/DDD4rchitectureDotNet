using Architecture.Domain.MessageBus.Outbox;

namespace Architecture.Application.MessageBus;

public static class EventBusExtensions
{
    public static Task PublishAsync<T>(this IEventBus eventBus, T @event, CancellationToken cancellationToken = default) where T : IIntegrationEvent
    {
        var integrationEvent = IntegrationEvent.Create(@event);
        return eventBus.PublishAsync(integrationEvent, cancellationToken);
    }
}
