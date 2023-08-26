using Architecture.Core;
using Architecture.Shell.CQRS;

namespace Architecture.Tests.Shell.CQRS
{
    public partial class MediatorExtensionsTests
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

        public class DomainEventA : IDomainEvent { };
        public class DomainEventB : IDomainEvent { };
        public class DomainEventC : IDomainEvent { };


        [Fact]
        public async Task 使用PublishAndClearDomainEvents_應該能夠發送()
        {
            // Given
            var aggregate = new SomethingAggregate();
            aggregate.DoSomething();
            var domainEvents = aggregate.DomainEvents.ToList();
            var mockEventMediator = new Mock<IMediator>();
            var mediator = mockEventMediator.Object;

            // When
            await mediator.PublishAndClearDomainEvents(aggregate);

            // Then
            mockEventMediator.Verify(m => m.PublishAsync(It.Is<SomethingDomainEvent>(e => domainEvents.Contains(e)), default), Times.Once());
        }

        [Fact]
        public async Task 使用PublishDomainEventsAsync時_應該能夠識別每個範型的PublishDomainEventAsync()
        {
            // Given
            var domainEvents = new IDomainEvent[] { new DomainEventA(), new DomainEventB(), new DomainEventC() };
            var mockEventMediator = new Mock<IMediator>();
            var mediator = mockEventMediator.Object;

            // When
            await mediator.PublishAsync(domainEvents);

            // Then
            mockEventMediator.Verify(m => m.PublishAsync(It.Is<DomainEventA>(e => domainEvents.Contains(e)), default), Times.Once());
            mockEventMediator.Verify(m => m.PublishAsync(It.Is<DomainEventB>(e => domainEvents.Contains(e)), default), Times.Once());
            mockEventMediator.Verify(m => m.PublishAsync(It.Is<DomainEventC>(e => domainEvents.Contains(e)), default), Times.Once());
        }
    }
}