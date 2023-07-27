using Architecture.Application.MessageBus.Outbox;
using Architecture.Domain.MessageBus.Outbox;

namespace Architecture.Tests.Application.MessageBus.Outbox;

public class OutboxProcessorTests
{
    [Fact]
    public void 進行Enqueue_應該會將TransactionId加至TransactionalQueue當中()
    {
        // Given
        var transactionalQueue = new TransactionalQueue(30);

        var repository = new Mock<IIntegrationEventRepository>();
        var eventBroker = new Mock<IEventBroker>();

        var outboxProcessor = new OutboxProcessor(transactionalQueue, repository.Object, eventBroker.Object);
        var transactionId = Guid.NewGuid();

        // When
        outboxProcessor.Enqueue(transactionId);

        // Then
        transactionalQueue.TransactionIds.Contains(transactionId).Should().BeTrue();
    }

    [Fact]
    public async Task 進行ProcessAsync_應該會將TransactionId代表的IntegrationEvent進行處理()
    {
        // Given
        var integrationEventEntry = GetIntegrationEventEntry();
        var transactionalQueue = new TransactionalQueue(30);

        var repository = new Mock<IIntegrationEventRepository>();
        repository.Setup(m => m.FindAsync(It.Is<IEnumerable<Guid>>(ids => ids.Contains(integrationEventEntry.TransactionId)), default)).ReturnsAsync(new List<IntegrationEventEntry> { integrationEventEntry });
        repository.Setup(m => m.SaveAsync(It.Is<IEnumerable<IntegrationEventEntry>>(e => e.Contains(integrationEventEntry)), default));
        repository.Setup(m => m.SaveAsync(It.Is<IEnumerable<IntegrationEventEntry>>(e => e.Contains(integrationEventEntry)), default));
        var eventBroker = new Mock<IEventBroker>();

        var outboxProcessor = new OutboxProcessor(transactionalQueue, repository.Object, eventBroker.Object);
        outboxProcessor.Enqueue(integrationEventEntry.TransactionId);

        // When
        await outboxProcessor.ProcessAsync(default);

        // Then
        transactionalQueue.TransactionIds.Should().BeEmpty();
        repository.Verify(m => m.SaveAsync(It.Is<IEnumerable<IntegrationEventEntry>>(e => e.Contains(integrationEventEntry)), default), Times.Exactly(2));
        eventBroker.Verify(m => m.PublishsAsync(It.Is<IEnumerable<IntegrationEvent>>(e => e.Contains(integrationEventEntry.GetIntegrationEvent())), default), Times.Once());
    }

    private static IntegrationEventEntry GetIntegrationEventEntry()
    {
        var transactionId = Guid.NewGuid();
        var somethingIntegrationEvent = new SomethingIntegrationEvent();
        var integrationEvent = IntegrationEvent.Create(somethingIntegrationEvent);
        var integrationEventEntry = IntegrationEventEntry.Raise(integrationEvent, transactionId);
        return integrationEventEntry;
    }
}
