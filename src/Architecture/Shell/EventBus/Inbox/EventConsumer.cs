using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Architecture.Shell.EventBus.Inbox;

public class EventConsumer : IEventConsumer
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<EventConsumer> _logger;

    public EventConsumer(IServiceProvider serviceProvider, ILogger<EventConsumer> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task ConsumeAsync<TIntegrationEvent>(TIntegrationEvent integrationEvent, CancellationToken cancellationToken = default) where TIntegrationEvent : IIntegrationEvent
    {
        var integrationEventType = integrationEvent.GetType();
        var integrationEventTypeName = integrationEventType.GetGenericTypeName();
        _logger.LogInformation("<<<<< Consume {IntegrationEventTypeName} {@IntegrationEvent}", integrationEventTypeName, integrationEvent);

        var handler = _serviceProvider.GetService<IIntegrationEventHandler<TIntegrationEvent>>();
        if (handler is null)
            return;
        await handler.HandleAsync(integrationEvent, cancellationToken);
    }
}
