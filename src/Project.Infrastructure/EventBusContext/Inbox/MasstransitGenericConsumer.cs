using Architecture.Shell.EventBus;
using Architecture.Shell.EventBus.Inbox;
using Hangfire;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Project.Infrastructure.EventBusContext.Inbox;

public class MasstransitGenericConsumer<T> : IConsumer<T> where T : class, IIntegrationEvent
{
    private readonly IInbox _inbox;
    private readonly IBackgroundJobClient _backgroundJobClient;
    private readonly ILogger<MasstransitGenericConsumer<T>> _logger;

    public MasstransitGenericConsumer(IInbox inbox, IBackgroundJobClient backgroundJobClient, ILogger<MasstransitGenericConsumer<T>> logger)
    {
        _inbox = inbox;
        _backgroundJobClient = backgroundJobClient;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<T> context)
    {
        var integrationEvent = context.Message;
        await _inbox.ConsumeAsync(integrationEvent, context.CancellationToken);

        _backgroundJobClient.Enqueue<IInboxWorker>(w => w.ProcessAsync(integrationEvent, CancellationToken.None));
        _logger.LogInformation("+++++ Inbox process integration event for {IntegrationEventId}", integrationEvent.Id);
    }
}
