using Architecture.Application.UnitOfWork;
using Microsoft.Extensions.Logging;

namespace Architecture.Application.EventBus.Outbox;

public class UnitOfWorkOutboxDecorator : IUnitOfWork
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IOutboxProcessor _outboxProcessor;
    private readonly ILogger<UnitOfWorkOutboxDecorator> _logger;

    public UnitOfWorkOutboxDecorator(IUnitOfWork unitOfWork, IOutboxProcessor outboxProcessor, ILogger<UnitOfWorkOutboxDecorator> logger)
    {
        _unitOfWork = unitOfWork;
        _outboxProcessor = outboxProcessor;
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

        _ = _outboxProcessor.ProcessAsync(transactionId, cancellationToken);
        _logger.LogInformation("+++++ Outbox process integration events for {TransactionId}", transactionId);
    }
}
