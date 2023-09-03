using Architecture.Core;
using Architecture.Shell.CQRS;

namespace Architecture.Tests.Shell.CQRS
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

    public class MediatorExtensionsTests
    {
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
            mockEventMediator.Verify(m => m.PublishAsync(It.Is<IDomainEvent>(e => e is SomethingDomainEvent && domainEvents.Contains(e)), default), Times.Once());
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
            mockEventMediator.Verify(m => m.PublishAsync(It.Is<IDomainEvent>(e => e is DomainEventA && domainEvents.Contains(e)), default), Times.Once());
            mockEventMediator.Verify(m => m.PublishAsync(It.Is<IDomainEvent>(e => e is DomainEventB && domainEvents.Contains(e)), default), Times.Once());
            mockEventMediator.Verify(m => m.PublishAsync(It.Is<IDomainEvent>(e => e is DomainEventC && domainEvents.Contains(e)), default), Times.Once());
        }
    }
}