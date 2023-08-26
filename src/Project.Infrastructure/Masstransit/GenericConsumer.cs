using Architecture.Shell.EventBus;
using Architecture.Shell.EventBus.Inbox;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Project.Infrastructure.Masstransit;

public class GenericConsumer<T> : IConsumer<T> where T : class, IIntegrationEvent
{
    private readonly IInbox _inbox;
    private readonly IInboxWorker _inboxProcessor;
    private readonly ILogger<GenericConsumer<T>> _logger;

    public GenericConsumer(IInbox inbox, IInboxWorker inboxProcessor, ILogger<GenericConsumer<T>> logger)
    {
        _inbox = inbox;
        _inboxProcessor = inboxProcessor;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<T> context)
    {
        var integrationEvent = context.Message;
        await _inbox.ConsumeAsync(integrationEvent, context.CancellationToken);
        _ = _inboxProcessor.ProcessAsync(integrationEvent.Id);
        _logger.LogInformation("+++++ Inbox process integration event for {IntegrationEventId}", integrationEvent.Id);
    }
}
