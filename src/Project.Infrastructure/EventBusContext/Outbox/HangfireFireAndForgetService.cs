using Architecture.Shell.EventBus.Outbox;
using Hangfire;

namespace Project.Infrastructure.EventBusContext.Outbox;

// NOTE: Inbox 的 Hangfire 直接寫在 MasstransitGenericConsumer 是因為 Outbox 的流程寫在 Architecture
public class HangfireFireAndForgetService : IFireAndForgetService
{
    private readonly IBackgroundJobClient _backgroundJobClient;

    public HangfireFireAndForgetService(IBackgroundJobClient backgroundJobClient)
    {
        _backgroundJobClient = backgroundJobClient;
    }

    public Task ExecuteAsync(Guid transactionId, CancellationToken cancellationToken = default)
    {
        _backgroundJobClient.Enqueue<IOutboxWorker>(w => w.ProcessAsync(transactionId, CancellationToken.None));
        return Task.CompletedTask;
    }
}
