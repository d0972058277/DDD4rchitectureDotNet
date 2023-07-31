using Architecture;
using Architecture.Application.EventBus;
using Architecture.Domain.EventBus;
using Microsoft.Extensions.Logging;

namespace Project.Infrastructure;

public class EventPublisher : IEventPublisher
{
    private readonly ILogger<EventPublisher> _logger;

    public EventPublisher(ILogger<EventPublisher> logger)
    {
        _logger = logger;
    }

    public Task PublishAsync<TIntegrationEvent>(TIntegrationEvent integrationEvent, CancellationToken cancellationToken = default) where TIntegrationEvent : IIntegrationEvent
    {
        var typeName = integrationEvent.GetGenericTypeName();
        _logger.LogInformation("Send {IntegrationEventTypeName} {@IntegrationEvent}", typeName, integrationEvent);
        return Task.CompletedTask;
    }
}
