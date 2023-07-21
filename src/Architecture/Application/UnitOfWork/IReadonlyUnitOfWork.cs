namespace Architecture.Application.UnitOfWork
{
    public interface IReadonlyUnitOfWork
    {
        Guid? TransactionId { get; }
        bool HasActiveTransaction { get; }
    }
}