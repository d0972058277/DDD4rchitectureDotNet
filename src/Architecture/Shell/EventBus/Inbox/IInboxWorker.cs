namespace Architecture.Shell.EventBus.Inbox;

public interface IInboxWorker
{
    Task ProcessAsync<TIntegrationEvent>(TIntegrationEvent integrationEvent, CancellationToken cancellationToken = default) where TIntegrationEvent : IIntegrationEvent;
}
