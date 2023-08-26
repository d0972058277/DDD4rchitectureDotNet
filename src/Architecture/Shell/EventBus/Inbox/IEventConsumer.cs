namespace Architecture.Shell.EventBus.Inbox;

public interface IEventConsumer
{
    Task ConsumeAsync<TIntegrationEvent>(TIntegrationEvent integrationEvent, CancellationToken cancellationToken = default) where TIntegrationEvent : IIntegrationEvent;
}
