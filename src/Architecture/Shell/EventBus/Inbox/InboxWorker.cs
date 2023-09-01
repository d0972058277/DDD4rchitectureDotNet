using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Architecture.Shell.EventBus.Inbox;

public class InboxWorker : IInboxWorker
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<InboxWorker> _logger;

    public InboxWorker(IServiceProvider serviceProvider, ILogger<InboxWorker> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task ProcessAsync<TIntegrationEvent>(TIntegrationEvent integrationEvent, CancellationToken cancellationToken = default) where TIntegrationEvent : IIntegrationEvent
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var repository = scope.ServiceProvider.GetRequiredService<IIntegrationEventRepository>();
            var eventConsumer = scope.ServiceProvider.GetRequiredService<IEventConsumer>();

            var entryFound = await repository.FindAsync(integrationEvent.Id, cancellationToken);
            if (entryFound.HasNoValue)
                return;

            var entry = entryFound.Value;

            entry.Progress();
            await repository.SaveAsync(entry, cancellationToken);

            await eventConsumer.ConsumeAsync(integrationEvent, cancellationToken);

            entry.Handle();
            await repository.SaveAsync(entry, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "InboxWorker caught an exception.");
        }
    }
}
