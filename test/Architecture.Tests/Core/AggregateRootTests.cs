namespace Architecture.Tests.Core
{
    public partial class AggregateRootTests
    {
        [Fact]
        public void 內部的方法使用AddDomainEvent後_應該能夠在DomainEvents中找到對應的DomainEvent()
        {
            // Given
            var aggregate = new SomethingAggregate();

            // When
            aggregate.SomethingMethod();

            // Then
            aggregate.DomainEvents.Should().Contain(e => e is SomethingDomainEvent);
        }

        [Fact]
        public void 應該能夠清除DomainEvents()
        {
            // Given
            var aggregate = new SomethingAggregate();
            aggregate.SomethingMethod();

            // When
            aggregate.ClearDomainEvents();

            // Then
            aggregate.DomainEvents.Should().BeEmpty();
        }
    }
}