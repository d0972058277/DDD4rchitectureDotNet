using Architecture.Domain.MessageBus;

namespace Architecture.Application.MessageBus
{
    public interface IEventBus
    {
        Task PublishAsync(IntegrationEvent integrationEvent, CancellationToken cancellationToken = default);
    }
}