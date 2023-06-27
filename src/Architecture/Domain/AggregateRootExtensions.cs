namespace Architecture.Domain
{
    public static class AggregateRootExtensions
    {
        public static IReadOnlyList<IDomainEvent> DumpDomainEvents(this IAggregateRoot aggregateRoot)
        {
            var domainEvents = aggregateRoot.DomainEvents;
            aggregateRoot.ClearDomainEvents();
            return domainEvents;
        }
    }
}