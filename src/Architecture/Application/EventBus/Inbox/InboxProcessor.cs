using Architecture.Domain.EventBus;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Architecture.Application.EventBus.Inbox;

public class InboxProcessor : IInboxProcessor
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<InboxProcessor> _logger;

    // TODO: 應該改寫 Func 的寫法，有其他更好的做法
    private readonly Func<IServiceProvider, Payload, Task> _consumeIntegrationEventsFunc;

    public InboxProcessor(IServiceProvider serviceProvider, ILogger<InboxProcessor> logger, Func<IServiceProvider, Payload, Task> consumeIntegrationEventsFunc)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _consumeIntegrationEventsFunc = consumeIntegrationEventsFunc;
    }

    public async Task ProcessAsync(Guid integrationEventId, CancellationToken cancellationToken = default)
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var repository = scope.ServiceProvider.GetRequiredService<IIntegrationEventRepository>();

            var entryFound = await repository.FindAsync(integrationEventId, cancellationToken);
            if (entryFound.HasNoValue)
                return;

            var entry = entryFound.Value;

            entry.Progress();
            await repository.SaveAsync(entry, cancellationToken);

            var payload = entry.GetPayload();
            await _consumeIntegrationEventsFunc(scope.ServiceProvider, payload);

            entry.Handle();
            await repository.SaveAsync(entry, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "InboxProcessor caught an exception.");
        }
    }
}
