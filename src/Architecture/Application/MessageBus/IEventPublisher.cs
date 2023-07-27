using Architecture.Domain.MessageBus;

namespace Architecture.Application.MessageBus
{
    public interface IEventPublisher
    {
        Task PublishAsync(IntegrationEvent integrationEvent, CancellationToken cancellationToken = default);
    }
}