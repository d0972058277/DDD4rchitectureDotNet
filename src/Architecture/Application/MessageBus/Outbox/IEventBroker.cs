using Architecture.Domain.MessageBus;

namespace Architecture.Application.MessageBus.Outbox;

public interface IEventBroker
{
    Task PublishsAsync(IEnumerable<IntegrationEvent> integrationEvents, CancellationToken cancellationToken = default);
}
