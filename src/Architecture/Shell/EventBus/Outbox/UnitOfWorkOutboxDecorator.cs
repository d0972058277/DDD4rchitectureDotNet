using Microsoft.Extensions.Logging;

namespace Architecture.Shell.EventBus.Outbox;

public class UnitOfWorkOutboxDecorator : IUnitOfWork
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IOutboxWorker _outboxWorker;
    private readonly ILogger<UnitOfWorkOutboxDecorator> _logger;

    public UnitOfWorkOutboxDecorator(IUnitOfWork unitOfWork, IOutboxWorker outboxWorker, ILogger<UnitOfWorkOutboxDecorator> logger)
    {
        _unitOfWork = unitOfWork;
        _outboxWorker = outboxWorker;
        _logger = logger;
    }

    public Guid? TransactionId => _unitOfWork.TransactionId;

    public bool HasActiveTransaction => _unitOfWork.HasActiveTransaction;

    public Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        return _unitOfWork.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        if (!_unitOfWork.HasActiveTransaction)
            throw new InvalidOperationException("UnitOfWork 應該要在有活躍的 Transaction 才可進行 CommitAsync");

        var transactionId = _unitOfWork.TransactionId!.Value;

        await _unitOfWork.CommitAsync(cancellationToken);

        // TODO: 這邊應該使用像是 Hangfire, Quartz.Net, Hosted Services 之類的進行非同步處理
        await _outboxWorker.ProcessAsync(transactionId, cancellationToken);
        _logger.LogInformation("+++++ Outbox process integration events for {TransactionId}", transactionId);
    }
}
