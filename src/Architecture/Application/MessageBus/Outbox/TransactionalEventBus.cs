namespace Architecture.Application.MessageBus.Outbox
{
    public class TransactionalEventBus : IEventBus
    {
        private readonly ITransactionalOutbox _transactionalOutbox;

        public TransactionalEventBus(ITransactionalOutbox transactionalOutbox)
        {
            _transactionalOutbox = transactionalOutbox;
        }

        public async Task PublishAsync<TIntegrationEvent>(TIntegrationEvent integrationEvent, CancellationToken cancellationToken = default) where TIntegrationEvent : IntegrationEvent
        {
            if (!_transactionalOutbox.UnitOfWork.HasActiveTransaction)
                throw new InvalidOperationException("IEventBus 的實作類 TransactionalEventBus 應該要在 IUnitOfWork 有活躍的 Transaction 才可進行整合事件發佈");

            await _transactionalOutbox.SaveAsync(integrationEvent, cancellationToken);
        }
    }
}