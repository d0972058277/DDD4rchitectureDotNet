namespace Architecture.Application.EventBus;

public interface IInboxProcessor
{
    Task ProcessAsync(Guid integrationEventId, CancellationToken cancellationToken = default);
}
