using Architecture.Domain;

namespace Architecture.Tests.Domain
{
    public partial class AggregateRootExtensionsTests
    {
        public class SomethingAggregate : AggregateRoot
        {
            public SomethingAggregate() : base(IdGenerator.NewId()) { }

            public void SomethingMethod()
            {
                AddDomainEvent(new SomethingDomainEvent());
            }
        }

        public class SomethingDomainEvent : IDomainEvent { }
    }
}