using Architecture.Domain.EventBus;

namespace Architecture.Application.EventBus;

public interface IEventInbox
{
    Task ReceiveAsync<T>(T @event, CancellationToken cancellationToken = default) where T : IIntegrationEvent;
}
