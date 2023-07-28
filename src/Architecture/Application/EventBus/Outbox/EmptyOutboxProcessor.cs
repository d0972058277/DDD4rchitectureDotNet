namespace Architecture.Application.EventBus.Outbox;

public class EmptyOutboxProcessor : IOutboxProcessor
{
    public Task ProcessAsync(Guid transactionId, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}
