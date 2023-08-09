using Architecture.Domain.EventBus;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Architecture.Application.EventBus.Inbox;

public class EventConsumer : IEventConsumer
{
    private readonly ILogger<EventConsumer> _logger;
    private readonly IServiceProvider _serviceProvider;

    public EventConsumer(ILogger<EventConsumer> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    public async Task ConsumeAsync<TIntegrationEvent>(TIntegrationEvent integrationEvent, CancellationToken cancellationToken = default) where TIntegrationEvent : IIntegrationEvent
    {
        var typeName = integrationEvent.GetGenericTypeName();
        _logger.LogInformation("<<<<< Consume {IntegrationEventTypeName} {@IntegrationEvent}", typeName, integrationEvent);

        var handlers = _serviceProvider.GetRequiredService<IEnumerable<IIntegrationEventHandler<TIntegrationEvent>>>();

        foreach (var handler in handlers)
        {
            /* 
            TODO: 更傾向能夠併發執行 IntegrationEventHandler
            但要考慮同一 Scope 會取得相同的 DbContext ，併發會出錯的問題
            有可能要有 IntegrationEventHandlerFactory ，並用 Dictionary 的方式紀錄 IntegrationEventHandler 的類型
            */
            await handler.HandleAsync(integrationEvent, cancellationToken);
        }
    }
}
