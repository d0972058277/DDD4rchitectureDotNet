namespace Architecture.Application.MessageBus.Outbox
{
    public class TransactionalEventBus : IEventBus
    {
        private readonly IOutboxTransactor _transactor;

        public TransactionalEventBus(IOutboxTransactor transactor)
        {
            _transactor = transactor;
        }

        public async Task PublishAsync<TIntegrationEvent>(TIntegrationEvent integrationEvent, CancellationToken cancellationToken = default) where TIntegrationEvent : IntegrationEvent
        {
            if (!_transactor.UnitOfWork.HasActiveTransaction)
                throw new InvalidOperationException("IEventBus 的實作類 TransactionalEventBus 應該要在 IUnitOfWork 有活躍的 Transaction 才可進行整合事件發佈");

            var transactionId = _transactor.UnitOfWork.TransactionId!.Value;
            await _transactor.SaveAsync(transactionId, integrationEvent, cancellationToken);
        }
    }
}