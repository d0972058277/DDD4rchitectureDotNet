using Architecture.Domain.EventBus;

namespace Architecture.Application.EventBus;

public interface IEventOutbox
{
    Task PublishAsync<TIntegrationEvent>(TIntegrationEvent integrationEvent, CancellationToken cancellationToken = default) where TIntegrationEvent : IIntegrationEvent;
}
