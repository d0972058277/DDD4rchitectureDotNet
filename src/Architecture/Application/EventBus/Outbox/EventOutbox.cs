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

        public Task SendAsync(IntegrationEvent integrationEvent, CancellationToken cancellationToken = default)
        {
            if (!_unitOfWork.HasActiveTransaction)
                throw new InvalidOperationException("IEventBus 的實作類 TransactionalEventBus 應該要在 IUnitOfWork 有活躍的 Transaction 才可進行整合事件發佈");

            var transactionId = _unitOfWork.TransactionId!.Value;
            var integrationEventEntry = IntegrationEventEntry.Raise(integrationEvent, transactionId);
            return _repository.AddAsync(integrationEventEntry, cancellationToken);
        }
    }
}