using Architecture.Domain.EventBus;

namespace Architecture.Application.EventBus;

public interface IEventConsumer
{
    Task ConsumeAsync<T>(T @event, CancellationToken cancellationToken = default) where T : IIntegrationEvent;
}
