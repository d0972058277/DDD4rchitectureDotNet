using Architecture.Domain.EventBus;

namespace Architecture.Application.EventBus;

public interface IEventInbox
{
    Task ReceiveAsync(IntegrationEvent integrationEvent, CancellationToken cancellationToken = default);
}
