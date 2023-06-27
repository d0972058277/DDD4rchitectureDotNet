using Architecture.Domain;

namespace Architecture.Tests.Domain
{
    public partial class AggregateRootExtensionsTests
    {
        [Fact]
        public void 使用Dump時_應該能夠取得DomainEvents且AggregateRoot的DomainEvents會被清除()
        {
            // Given
            var aggregate = new SomethingAggregate();
            aggregate.SomethingMethod();
            var expectDomainEvents = aggregate.DomainEvents;

            // When
            var actualDomainEvents = aggregate.DumpDomainEvents();

            // Then
            actualDomainEvents.Should().BeEquivalentTo(expectDomainEvents);
            aggregate.DomainEvents.Should().BeEmpty();
        }
    }
}