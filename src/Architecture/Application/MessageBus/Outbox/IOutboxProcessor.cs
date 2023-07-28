namespace Architecture.Application.MessageBus.Outbox;

public interface IOutboxProcessor
{
    Task ProcessAsync(Guid transactionId, CancellationToken cancellationToken = default);
}
