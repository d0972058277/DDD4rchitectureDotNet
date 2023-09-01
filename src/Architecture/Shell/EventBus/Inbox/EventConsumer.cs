using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Architecture.Shell.EventBus.Inbox;

public class EventConsumer : IEventConsumer
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<EventConsumer> _logger;

    public EventConsumer(IServiceScopeFactory serviceScopeFactory, ILogger<EventConsumer> logger)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
    }

    public async Task ConsumeAsync<TIntegrationEvent>(TIntegrationEvent integrationEvent, CancellationToken cancellationToken = default) where TIntegrationEvent : IIntegrationEvent
    {
        var integrationEventType = integrationEvent.GetType();
        var integrationEventTypeName = integrationEventType.GetGenericTypeName();

        _logger.LogInformation("<<<<< Consume {IntegrationEventTypeName} {@IntegrationEvent}", integrationEventTypeName, integrationEvent);

        using var scope = _serviceScopeFactory.CreateScope();
        var handler = scope.ServiceProvider.GetRequiredService<IIntegrationEventHandler<TIntegrationEvent>>();
        await handler.HandleAsync(integrationEvent, cancellationToken);
    }
}
