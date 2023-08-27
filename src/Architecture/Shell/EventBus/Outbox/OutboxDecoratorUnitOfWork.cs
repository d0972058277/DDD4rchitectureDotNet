using Microsoft.Extensions.Logging;

namespace Architecture.Shell.EventBus.Outbox;

public class OutboxDecoratorUnitOfWork : IUnitOfWork
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFireAndForgetService _fireAndForgetService;
    private readonly ILogger<OutboxDecoratorUnitOfWork> _logger;

    public OutboxDecoratorUnitOfWork(IUnitOfWork unitOfWork, IFireAndForgetService fireAndForgetService, ILogger<OutboxDecoratorUnitOfWork> logger)
    {
        _unitOfWork = unitOfWork;
        _fireAndForgetService = fireAndForgetService;
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

        await _fireAndForgetService.ExecuteAsync(transactionId, cancellationToken);

        _logger.LogInformation("+++++ Outbox process integration events for {TransactionId}", transactionId);
    }
}
