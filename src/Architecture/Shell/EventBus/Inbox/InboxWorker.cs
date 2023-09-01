using Microsoft.Extensions.Logging;

namespace Architecture.Shell.EventBus.Inbox;

public class InboxWorker : IInboxWorker
{
    private readonly IIntegrationEventRepository _repository;
    private readonly IEventConsumer _eventConsumer;
    private readonly ILogger<InboxWorker> _logger;

    public InboxWorker(IIntegrationEventRepository repository, IEventConsumer eventConsumer, ILogger<InboxWorker> logger)
    {
        _repository = repository;
        _eventConsumer = eventConsumer;
        _logger = logger;
    }

    public async Task ProcessAsync<TIntegrationEvent>(TIntegrationEvent integrationEvent, CancellationToken cancellationToken = default) where TIntegrationEvent : IIntegrationEvent
    {
        try
        {
            var entryFound = await _repository.FindAsync(integrationEvent.Id, cancellationToken);
            if (entryFound.HasNoValue)
                return;

            await _eventConsumer.ConsumeAsync(integrationEvent, cancellationToken);

            var entry = entryFound.Value;
            entry.Handle();
            await _repository.SaveAsync(entry, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "InboxWorker caught an exception.");
        }
    }
}
