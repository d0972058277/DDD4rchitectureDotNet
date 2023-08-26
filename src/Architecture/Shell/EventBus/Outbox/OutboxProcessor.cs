using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Architecture.Shell.EventBus.Outbox;

public class OutboxWorker : IOutboxWorker
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<OutboxWorker> _logger;

    public OutboxWorker(IServiceProvider serviceProvider, ILogger<OutboxWorker> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task ProcessAsync(Guid transactionId, CancellationToken cancellationToken = default)
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var repository = scope.ServiceProvider.GetRequiredService<IIntegrationEventRepository>();
            var eventPublisher = scope.ServiceProvider.GetRequiredService<IEventPublisher>();

            var entries = await repository.FindAsync(transactionId, cancellationToken);

            if (!entries.Any())
                return;

            foreach (var entry in entries)
                entry.Progress();
            await repository.SaveAsync(entries, cancellationToken);

            var integrationEvents = entries.Select(e => e.GetPayload().Deserialize()).ToList();
            foreach (var integrationEvent in integrationEvents)
                await eventPublisher.PublishAsync(integrationEvent, cancellationToken);

            foreach (var entry in entries)
                entry.Publish();
            await repository.SaveAsync(entries, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "OutboxWorker caught an exception.");
        }
    }
}
