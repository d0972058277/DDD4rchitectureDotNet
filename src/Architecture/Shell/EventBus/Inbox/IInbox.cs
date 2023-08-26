namespace Architecture.Shell.EventBus.Inbox;

public interface IInbox
{
    Task ConsumeAsync<TIntegrationEvent>(TIntegrationEvent integrationEvent, CancellationToken cancellationToken = default) where TIntegrationEvent : IIntegrationEvent;
}
