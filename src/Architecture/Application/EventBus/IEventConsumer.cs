using Architecture.Domain.EventBus;

namespace Architecture.Application.EventBus;

public interface IEventConsumer
{
    Task ConsumeAsync<TIntegrationEvent>(TIntegrationEvent integrationEvent, CancellationToken cancellationToken = default) where TIntegrationEvent : IIntegrationEvent;
}
