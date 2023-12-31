namespace Architecture.Shell
{
    public interface IUnitOfWork
    {
        Guid? TransactionId { get; }
        bool HasActiveTransaction { get; }
        Task BeginTransactionAsync(CancellationToken cancellationToken = default);
        Task CommitAsync(CancellationToken cancellationToken = default);
    }
}