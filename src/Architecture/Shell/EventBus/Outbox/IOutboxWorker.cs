namespace Architecture.Shell.EventBus.Outbox;

public interface IOutboxWorker
{
    Task ProcessAsync(Guid transactionId, CancellationToken cancellationToken = default);
}
