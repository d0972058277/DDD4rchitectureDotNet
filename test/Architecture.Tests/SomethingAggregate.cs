using Architecture.Domain;

namespace Architecture.Tests;

public class SomethingAggregate : AggregateRoot
{
    public SomethingAggregate() : base(IdGenerator.NewId()) { }

    public void SomethingMethod()
    {
        AddDomainEvent(new SomethingDomainEvent());
    }
}
