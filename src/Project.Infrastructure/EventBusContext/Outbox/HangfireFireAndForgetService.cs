using Architecture.Shell.EventBus.Outbox;
using Hangfire;

namespace Project.Infrastructure.EventBusContext.Outbox;

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
