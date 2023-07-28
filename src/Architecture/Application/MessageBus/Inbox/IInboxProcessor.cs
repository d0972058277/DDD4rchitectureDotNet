namespace Architecture.Application.MessageBus.Inbox;

public interface IInboxProcessor
{
    Task ProcessAsync(Guid integrationEventId, CancellationToken cancellationToken = default);
}
