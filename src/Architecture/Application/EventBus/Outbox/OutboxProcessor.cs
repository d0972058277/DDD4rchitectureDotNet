using Architecture.Domain.EventBus;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Architecture.Application.EventBus.Outbox;

public class OutboxProcessor : IOutboxProcessor
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<OutboxProcessor> _logger;
    private readonly Func<IServiceProvider, IEnumerable<Payload>, Task> _publishIntegrationEventsFunc;

    public OutboxProcessor(IServiceProvider serviceProvider, ILogger<OutboxProcessor> logger, Func<IServiceProvider, IEnumerable<Payload>, Task> publishIntegrationEventsFunc)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _publishIntegrationEventsFunc = publishIntegrationEventsFunc;
    }

    public async Task ProcessAsync(Guid transactionId, CancellationToken cancellationToken = default)
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var repository = scope.ServiceProvider.GetRequiredService<IIntegrationEventRepository>();

            var entries = await repository.FindAsync(transactionId, cancellationToken);

            if (!entries.Any())
                return;

            foreach (var entry in entries)
                entry.Progress();
            await repository.SaveAsync(entries, cancellationToken);

            var payloads = entries.Select(e => e.GetPayload()).ToList();
            await _publishIntegrationEventsFunc(scope.ServiceProvider, payloads);

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
