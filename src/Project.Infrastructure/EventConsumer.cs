using Architecture;
using Architecture.Application.EventBus;
using Architecture.Domain.EventBus;
using Microsoft.Extensions.Logging;

namespace Project.Infrastructure;

public class EventConsumer : IEventConsumer
{
    private readonly ILogger<EventConsumer> _logger;

    public EventConsumer(ILogger<EventConsumer> logger)
    {
        _logger = logger;
    }

    public Task ConsumeAsync<TIntegrationEvent>(TIntegrationEvent integrationEvent, CancellationToken cancellationToken = default) where TIntegrationEvent : IIntegrationEvent
    {
        var typeName = integrationEvent.GetGenericTypeName();
        _logger.LogInformation("<<<<< Consume {IntegrationEventTypeName} {@IntegrationEvent}", typeName, integrationEvent);

        // TODO: 有沒有必要創建 IntegrationEventHandler 與其 Mediator ？或是直接透過 Integration.ToCommand() 做使用？

        return Task.CompletedTask;
    }
}
