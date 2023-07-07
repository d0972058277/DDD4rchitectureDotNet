using Architecture.Application.UnitOfWork;

namespace Architecture.Application.MessageBus.Outbox
{
    public interface IOutboxTransactor
    {
        IReadonlyUnitOfWork UnitOfWork { get; }
        Task SaveAsync<TIntegrationEvent>(Guid transactionId, TIntegrationEvent integrationEvent, CancellationToken cancellationToken = default) where TIntegrationEvent : IntegrationEvent;
        Task CommitAsync(Guid transactionId, CancellationToken cancellationToken = default);
    }
}