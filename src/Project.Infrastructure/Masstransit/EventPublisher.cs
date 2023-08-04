using Architecture;
using Architecture.Application.EventBus;
using Architecture.Domain.EventBus;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Project.Infrastructure.Masstransit;

public class EventPublisher : IEventPublisher
{
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<EventPublisher> _logger;

    public EventPublisher(IPublishEndpoint publishEndpoint, ILogger<EventPublisher> logger)
    {
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }

    public Task PublishAsync<TIntegrationEvent>(TIntegrationEvent integrationEvent, CancellationToken cancellationToken = default) where TIntegrationEvent : IIntegrationEvent
    {
        var typeName = integrationEvent.GetGenericTypeName();
        _logger.LogInformation(">>>>> Publish {IntegrationEventTypeName} {@IntegrationEvent}", typeName, integrationEvent);
        // TODO: 要加入 CorrelationId 的動作
        return _publishEndpoint.Publish(integrationEvent, cancellationToken);
    }
}
