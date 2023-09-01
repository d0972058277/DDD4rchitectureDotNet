using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Architecture.Shell.EventBus.Inbox;

public class EventConsumer : IEventConsumer
{
    private readonly IIntegrationEventHandlerTypeCache _integrationEventHandlerTypeCache;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<EventConsumer> _logger;

    public EventConsumer(IIntegrationEventHandlerTypeCache integrationEventHandlerTypeCache, IServiceScopeFactory serviceScopeFactory, ILogger<EventConsumer> logger)
    {
        _integrationEventHandlerTypeCache = integrationEventHandlerTypeCache;
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
    }

    public async Task ConsumeAsync<TIntegrationEvent>(TIntegrationEvent integrationEvent, CancellationToken cancellationToken = default) where TIntegrationEvent : IIntegrationEvent
    {
        var integrationEventType = integrationEvent.GetType();
        var integrationEventTypeName = integrationEventType.GetGenericTypeName();

        _logger.LogInformation("<<<<< Consume {IntegrationEventTypeName} {@IntegrationEvent}", integrationEventTypeName, integrationEvent);

        var handlerTypes = _integrationEventHandlerTypeCache.GetImpHandlerTypes(integrationEventType);

        var tasks = handlerTypes.Select(async handlerType =>
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var handler = (IIntegrationEventHandler<TIntegrationEvent>)scope.ServiceProvider.GetRequiredService(handlerType);
            await handler.HandleAsync(integrationEvent, cancellationToken);
        }).ToList();

        await Task.WhenAll(tasks);
    }
}
