using Microsoft.Extensions.Logging;

namespace Architecture.Shell.EventBus.Outbox;

public class OutboxWorker : IOutboxWorker
{
    private readonly IIntegrationEventRepository _repository;
    private readonly IEventPublisher _eventPublisher;
    private readonly ILogger<OutboxWorker> _logger;

    public OutboxWorker(IIntegrationEventRepository repository, IEventPublisher eventPublisher, ILogger<OutboxWorker> logger)
    {
        _repository = repository;
        _eventPublisher = eventPublisher;
        _logger = logger;
    }

    public async Task ProcessAsync(Guid transactionId, CancellationToken cancellationToken = default)
    {
        try
        {
            var entries = await _repository.FindAsync(transactionId, cancellationToken);

            if (!entries.Any())
                return;

            var integrationEvents = entries.Select(e => e.GetPayload().Deserialize()).ToList();
            foreach (var integrationEvent in integrationEvents)
                await _eventPublisher.PublishAsync(integrationEvent, cancellationToken);

            foreach (var entry in entries)
                entry.Publish();
            await _repository.SaveAsync(entries, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "OutboxWorker caught an exception.");
            throw;
        }
    }
}
