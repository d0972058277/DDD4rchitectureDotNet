using Architecture.Domain;

namespace Architecture.Tests.Application.CQRS
{
    public partial class EventMediatorExtensionsTests
    {
        public class SomethingAggregate : AggregateRoot
        {
            public SomethingAggregate() : base(IdGenerator.NewId()) { }

            public void DoSomething()
            {
                AddDomainEvent(new SomethingDomainEvent());
            }
        }

        public class SomethingDomainEvent : IDomainEvent { }
    }
}