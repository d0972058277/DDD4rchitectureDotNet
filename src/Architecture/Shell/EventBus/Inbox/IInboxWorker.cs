namespace Architecture.Shell.EventBus.Inbox;

public interface IInboxWorker
{
    Task ProcessAsync(Guid integrationEventId, CancellationToken cancellationToken = default);
}
