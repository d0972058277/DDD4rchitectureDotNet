using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Architecture.Application.EventBus.Outbox;

public class OutboxProcessor : IOutboxProcessor
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<OutboxProcessor> _logger;

    public OutboxProcessor(IServiceProvider serviceProvider, ILogger<OutboxProcessor> logger)
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
            var eventOutboxFactory = scope.ServiceProvider.GetRequiredService<IEventOutboxFactory>();
            var eventOutbox = eventOutboxFactory.GetRealityEventOutbox();

            var entries = await repository.FindAsync(transactionId, cancellationToken);

            if (!entries.Any())
                return;

            foreach (var entry in entries)
                entry.Progress();
            await repository.SaveAsync(entries, cancellationToken);

            var integrationEvents = entries.Select(e => e.GetPayload().Deserialize()).ToList();
            foreach (var integrationEvent in integrationEvents)
                await eventOutbox.SendAsync(integrationEvent, cancellationToken);

            foreach (var entry in entries)
                entry.Publish();
            await repository.SaveAsync(entries, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "OutboxProcessor caught an exception.");
        }
    }
}
