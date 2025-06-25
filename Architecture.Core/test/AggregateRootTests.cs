using Shouldly;

namespace Architecture.Core.Test;

public partial class AggregateRootTests
{
    [Fact]
    public void 內部的方法使用AddDomainEvent後_應該能夠在DomainEvents中找到對應的DomainEvent()
    {
        // Given
        var aggregate = new SomethingAggregate(Guid.NewGuid());

        // When
        aggregate.SomethingMethod();

        // Then
        aggregate.DomainEvents.ShouldContain(e => e is SomethingDomainEvent);
    }

    [Fact]
    public void 應該能夠清除DomainEvents()
    {
        // Given
        var aggregate = new SomethingAggregate(Guid.NewGuid());
        aggregate.SomethingMethod();

        // When
        aggregate.ClearDomainEvents();

        // Then
        aggregate.DomainEvents.ShouldBeEmpty();
    }

    public class SomethingAggregate : AggregateRoot
    {
        public SomethingAggregate(Guid id) : base(id) { }

        public void SomethingMethod()
        {
            AddDomainEvent(new SomethingDomainEvent());
        }
    }

    public class SomethingDomainEvent : IDomainEvent
    {
    }
}