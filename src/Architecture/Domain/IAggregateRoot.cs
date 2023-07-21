namespace Architecture.Domain
{
    public interface IAggregateRoot
    {
        IReadOnlyList<IDomainEvent> DomainEvents { get; }
        void ClearDomainEvents();
    }
}