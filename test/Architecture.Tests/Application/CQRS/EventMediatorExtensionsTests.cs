using Architecture.Application.CQRS;
using Architecture.Domain;

namespace Architecture.Tests.Application.CQRS
{
    public class EventMediatorExtensionsTests
    {
        public class DomainEventA : IDomainEvent { };
        public class DomainEventB : IDomainEvent { };
        public class DomainEventC : IDomainEvent { };

        [Fact]
        public async Task 使用PublishDomainEventsAsync時_應該能夠識別每個範型的PublishDomainEventAsync()
        {
            // Given
            var domainEvents = new IDomainEvent[] { new DomainEventA(), new DomainEventB(), new DomainEventC() };
            var mockEventMediator = new Mock<IEventMediator>();
            var eventMediator = mockEventMediator.Object;

            // When
            await eventMediator.PublishAsync(domainEvents);

            // Then
            mockEventMediator.Verify(m => m.PublishAsync(It.IsAny<DomainEventA>(), default), Times.Once());
            mockEventMediator.Verify(m => m.PublishAsync(It.IsAny<DomainEventB>(), default), Times.Once());
            mockEventMediator.Verify(m => m.PublishAsync(It.IsAny<DomainEventC>(), default), Times.Once());
        }
    }
}