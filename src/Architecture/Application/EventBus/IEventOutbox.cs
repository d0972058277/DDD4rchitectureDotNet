using Architecture.Domain.EventBus;

namespace Architecture.Application.EventBus;

public interface IEventOutbox
{
    Task SendAsync(IntegrationEvent integrationEvent, CancellationToken cancellationToken = default);
}