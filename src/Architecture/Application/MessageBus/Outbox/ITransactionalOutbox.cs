using Architecture.Application.UnitOfWork;

namespace Architecture.Application.MessageBus.Outbox
{
    public interface ITransactionalOutbox
    {
        IUnitOfWork UnitOfWork { get; }
        Task SaveAsync<TIntegrationEvent>(TIntegrationEvent integrationEvent, CancellationToken cancellationToken = default) where TIntegrationEvent : IntegrationEvent;
        Task PublishAsync(CancellationToken cancellationToken = default);
    }
}