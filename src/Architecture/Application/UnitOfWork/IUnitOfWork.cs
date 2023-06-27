namespace Architecture.Application.UnitOfWork
{
    public interface IUnitOfWork
    {
        Guid? TransactionId { get; }
        bool HasActiveTransaction { get; }
        Task<Guid> BeginTransactionAsync(CancellationToken cancellationToken = default);
        Task CommitAsync(CancellationToken cancellationToken = default);
        Task RollbackAsync(CancellationToken cancellationToken = default);
    }
}