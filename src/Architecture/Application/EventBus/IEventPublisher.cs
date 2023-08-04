using Architecture.Domain.EventBus;

namespace Architecture.Application.EventBus;

public interface IEventPublisher
{
    Task PublishAsync<TIntegrationEvent>(TIntegrationEvent integrationEvent, CancellationToken cancellationToken = default) where TIntegrationEvent : IIntegrationEvent;
}