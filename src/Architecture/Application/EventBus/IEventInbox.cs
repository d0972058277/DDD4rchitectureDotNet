using Architecture.Domain.EventBus;

namespace Architecture.Application.EventBus;

public interface IEventInbox
{
    Task ConsumeAsync<TIntegrationEvent>(TIntegrationEvent integrationEvent, CancellationToken cancellationToken = default) where TIntegrationEvent : IIntegrationEvent;
}
