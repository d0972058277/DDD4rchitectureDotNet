using Architecture.Shell;

namespace Project.Infrastructure;

public class UnitOfWork : IUnitOfWork
{
    private readonly ProjectDbContext _dbContext;

    public UnitOfWork(ProjectDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Guid? TransactionId { get; private set; }

    public bool HasActiveTransaction => TransactionId.HasValue;

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        var dbContextTransaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
        TransactionId = dbContextTransaction.TransactionId;
    }

    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        await _dbContext.SaveChangesAsync(cancellationToken);
        await _dbContext.Database.CommitTransactionAsync(cancellationToken);
        TransactionId = null;
    }
}
