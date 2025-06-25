namespace Architecture.Core
{
    public interface IDomainEventHandler<in TDomainEvent> where TDomainEvent : IDomainEvent { }
}