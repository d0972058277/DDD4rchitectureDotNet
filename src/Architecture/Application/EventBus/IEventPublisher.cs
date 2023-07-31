using Architecture.Domain.EventBus;

namespace Architecture.Application.EventBus;

public interface IEventPublisher
{
    Task PublishAsync<T>(T @event, CancellationToken cancellationToken = default) where T : IIntegrationEvent;
}