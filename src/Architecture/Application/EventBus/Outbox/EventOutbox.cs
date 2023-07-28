using Architecture.Application.UnitOfWork;
using Architecture.Domain.EventBus;
using Architecture.Domain.EventBus.Outbox;

namespace Architecture.Application.EventBus.Outbox
{
    public class EventOutbox : IEventOutbox
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IIntegrationEventRepository _repository;

        public EventOutbox(IUnitOfWork unitOfWork, IIntegrationEventRepository repository)
        {
            _unitOfWork = unitOfWork;
            _repository = repository;
        }

        public Task SendAsync<T>(T @event, CancellationToken cancellationToken = default) where T : IIntegrationEvent
        {
            if (!_unitOfWork.HasActiveTransaction)
                throw new InvalidOperationException("IEventBus 的實作類 TransactionalEventBus 應該要在 IUnitOfWork 有活躍的 Transaction 才可進行整合事件發佈");

            var transactionId = _unitOfWork.TransactionId!.Value;
            var payload = Payload.Serialize(@event);
            var entry = IntegrationEventEntry.Raise(payload, transactionId);
            return _repository.AddAsync(entry, cancellationToken);
        }
    }
}