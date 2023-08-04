using Architecture.Application.EventBus;
using Architecture.Domain.EventBus;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Project.Infrastructure.Masstransit;

public class GenericConsumer<T> : IConsumer<T> where T : class, IIntegrationEvent
{
    private readonly IEventInbox _eventInbox;
    private readonly IInboxProcessor _inboxProcessor;
    private readonly ILogger<GenericConsumer<T>> _logger;

    public GenericConsumer(IEventInbox eventInbox, IInboxProcessor inboxProcessor, ILogger<GenericConsumer<T>> logger)
    {
        _eventInbox = eventInbox;
        _inboxProcessor = inboxProcessor;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<T> context)
    {
        var integrationEvent = context.Message;
        await _eventInbox.ConsumeAsync(integrationEvent, context.CancellationToken);
        _ = _inboxProcessor.ProcessAsync(integrationEvent.Id);
        _logger.LogInformation("+++++ Inbox process integration event for {IntegrationEventId}", integrationEvent.Id);
    }
}
