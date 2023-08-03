using Architecture.Domain.EventBus;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Architecture.Application.EventBus.Inbox;

public class InboxProcessor : IInboxProcessor
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<InboxProcessor> _logger;

    public InboxProcessor(IServiceProvider serviceProvider, ILogger<InboxProcessor> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task ProcessAsync(Guid integrationEventId, CancellationToken cancellationToken = default)
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var repository = scope.ServiceProvider.GetRequiredService<IIntegrationEventRepository>();
            var eventConsumer = scope.ServiceProvider.GetRequiredService<IEventConsumer>();

            var entryFound = await repository.FindAsync(integrationEventId, cancellationToken);
            if (entryFound.HasNoValue)
                return;

            var entry = entryFound.Value;

            entry.Progress();
            await repository.SaveAsync(entry, cancellationToken);

            var integrationEvent = entry.GetPayload().Deserialize();
            await eventConsumer.ConsumeAsync(integrationEvent, cancellationToken);

            entry.Handle();
            await repository.SaveAsync(entry, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "InboxProcessor caught an exception.");
        }
    }
}
