using Architecture.Domain.MessageBus;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Architecture.Application.MessageBus.Inbox;

public class InboxProcessor
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<InboxProcessor> _logger;
    private readonly Func<IServiceScope, IntegrationEvent, Task> _consumeIntegrationEventsFunc;

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

            var entry = await repository.FindAsync(integrationEventId, cancellationToken);

            entry.Progress();
            await repository.SaveAsync(entry, cancellationToken);

            var integrationEvent = entry.GetIntegrationEvent();
            await _consumeIntegrationEventsFunc(scope, integrationEvent);

            entry.Consume();
            await repository.SaveAsync(entry, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "InboxProcessor caught an exception.");
        }
    }
}
