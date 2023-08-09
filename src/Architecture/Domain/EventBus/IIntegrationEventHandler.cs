namespace Architecture.Domain.EventBus;

public interface IIntegrationEventHandler<T> where T : IIntegrationEvent
{
    Task HandleAsync(T integrationEvent, CancellationToken cancellationToken);
}
