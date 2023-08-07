using Architecture;
using Architecture.Application.EventBus;
using Architecture.Application.Services.Correlation;
using Architecture.Domain.EventBus;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Project.Infrastructure.Masstransit;

public class EventPublisher : IEventPublisher
{
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ICorrelationService _correlationService;
    private readonly ILogger<EventPublisher> _logger;

    public EventPublisher(IPublishEndpoint publishEndpoint, ICorrelationService correlationService, ILogger<EventPublisher> logger)
    {
        _publishEndpoint = publishEndpoint;
        _correlationService = correlationService;
        _logger = logger;
    }

    public Task PublishAsync<TIntegrationEvent>(TIntegrationEvent integrationEvent, CancellationToken cancellationToken = default) where TIntegrationEvent : IIntegrationEvent
    {
        var typeName = integrationEvent.GetGenericTypeName();
        _logger.LogInformation(">>>>> Publish {IntegrationEventTypeName} {@IntegrationEvent}", typeName, integrationEvent);
        return _publishEndpoint.Publish(integrationEvent, ctx => ctx.CorrelationId = _correlationService.CorrelationId, cancellationToken);
    }
}
