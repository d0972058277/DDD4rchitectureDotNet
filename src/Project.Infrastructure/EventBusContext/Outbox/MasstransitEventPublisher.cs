using Architecture;
using Architecture.Shell.Correlation;
using Architecture.Shell.EventBus;
using Architecture.Shell.EventBus.Outbox;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Project.Infrastructure.EventBusContext.Outbox;

public class MasstransitEventPublisher : IEventPublisher
{
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ICorrelationService _correlationService;
    private readonly ILogger<MasstransitEventPublisher> _logger;

    public MasstransitEventPublisher(IPublishEndpoint publishEndpoint, ICorrelationService correlationService, ILogger<MasstransitEventPublisher> logger)
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
