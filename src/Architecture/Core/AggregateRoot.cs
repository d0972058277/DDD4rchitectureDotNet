using CSharpFunctionalExtensions;

namespace Architecture.Core
{
    public abstract class AggregateRoot<TId> : Entity<TId>, IAggregateRoot where TId : IComparable<TId>
    {
        protected AggregateRoot() : base() { }
        protected AggregateRoot(TId id) : base(id) { }

        private readonly List<IDomainEvent> _domainEvents = new();
        public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        protected void AddDomainEvent(IDomainEvent domainEvent)
        {
            _domainEvents.Add(domainEvent);
        }

        public void ClearDomainEvents()
        {
            _domainEvents.Clear();
        }
    }

    public abstract class AggregateRoot : AggregateRoot<Guid>
    {
        protected AggregateRoot(Guid id) : base(id) { }
    }
}