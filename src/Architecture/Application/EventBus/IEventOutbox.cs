using Architecture.Domain.EventBus;

namespace Architecture.Application.EventBus;

public interface IEventOutbox
{
    Task SendAsync<T>(T @event, CancellationToken cancellationToken = default) where T : IIntegrationEvent;
}