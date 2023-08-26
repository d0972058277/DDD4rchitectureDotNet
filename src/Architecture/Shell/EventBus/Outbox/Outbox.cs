namespace Architecture.Shell.EventBus.Outbox
{
    public class Outbox : IOutbox
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IIntegrationEventRepository _repository;

        public Outbox(IUnitOfWork unitOfWork, IIntegrationEventRepository repository)
        {
            _unitOfWork = unitOfWork;
            _repository = repository;
        }

        public Task PublishAsync<TIntegrationEvent>(TIntegrationEvent integrationEvent, CancellationToken cancellationToken = default) where TIntegrationEvent : IIntegrationEvent
        {
            if (!_unitOfWork.HasActiveTransaction)
                throw new InvalidOperationException($"{nameof(IOutbox)} 的實作類 {nameof(Outbox)} 應該要在 IUnitOfWork 有活躍的 Transaction 才可進行整合事件發佈");

            var transactionId = _unitOfWork.TransactionId!.Value;
            var payload = Payload.Serialize(integrationEvent);
            var entry = IntegrationEventEntity.Raise(payload, transactionId);
            return _repository.AddAsync(entry, cancellationToken);
        }
    }
}