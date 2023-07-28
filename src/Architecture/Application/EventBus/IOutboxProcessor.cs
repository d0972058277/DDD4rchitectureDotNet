namespace Architecture.Application.EventBus;

public interface IOutboxProcessor
{
    Task ProcessAsync(Guid transactionId, CancellationToken cancellationToken = default);
}
