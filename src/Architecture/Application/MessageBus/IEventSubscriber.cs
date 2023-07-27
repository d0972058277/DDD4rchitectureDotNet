using Architecture.Domain.MessageBus;

namespace Architecture.Application.MessageBus;

public interface IEventSubscriber
{
    Task SubscribeAsync(IntegrationEvent integrationEvent, CancellationToken cancellationToken = default);
}
