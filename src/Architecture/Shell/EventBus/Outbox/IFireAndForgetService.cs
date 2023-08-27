namespace Architecture.Shell.EventBus.Outbox;

public interface IFireAndForgetService
{
    Task ExecuteAsync(Guid transactionId, CancellationToken cancellationToken = default);
}
