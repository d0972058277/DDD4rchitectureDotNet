using Architecture.Domain.MessageBus.Outbox;

namespace Architecture.Application.MessageBus
{
    public interface IEventBus
    {
        Task PublishAsync(IntegrationEvent integrationEvent, CancellationToken cancellationToken = default);
    }
}