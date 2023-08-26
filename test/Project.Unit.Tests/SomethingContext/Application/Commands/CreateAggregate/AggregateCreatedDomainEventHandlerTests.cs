using Architecture.Shell.EventBus.Outbox;
using Moq;
using Project.Application.SomethingContext.DomainEventHandlers;
using Project.Application.SomethingContext.IntegrationEvents;
using Project.Domain.SomethingContext.Events;

namespace Project.Unit.Tests.SomethingContext.Application.Commands.CreateAggregate;

public class AggregateCreatedDomainEventHandlerTests
{
    [Fact]
    public async Task 應該發送AggregateCreatedIntegrationEvent()
    {
        // Given
        var id = Guid.NewGuid();
        var outbox = new Mock<IOutbox>();

        var domainEvent = new AggregateCreatedDomainEvent(id);
        var handler = new AggregateCreatedDomainEventHandler(outbox.Object);

        // When
        await handler.Handle(domainEvent, default);

        // Then
        outbox.Verify(m => m.PublishAsync(It.Is<AggregateCreatedIntegrationEvent>(e => e.SomethingAggregateId == domainEvent.SomethingAggregateId), default), Times.Once());
    }
}
