using Architecture.Shell.EventBus;
using Architecture.Shell.EventBus.Outbox;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Architecture.Tests.Shell.EventBus.Outbox;

public class OutboxWorkerTests
{
    [Fact]
    public async Task 應該會選取TransactionId對應的Events操作Progress_接著執行發佈Events的Func_最後操作Publish()
    {
        // Given
        var entry = GetIntegrationEventEntity();
        var entries = new List<IntegrationEventEntity> { entry };
        var payloads = entries.Select(e => e.GetPayload()).ToList();
        var transactionId = entry.TransactionId;

        var serviceProvider = new Mock<IServiceProvider>();
        var serviceScopeFactory = new Mock<IServiceScopeFactory>();
        var serviceScope = new Mock<IServiceScope>();

        serviceProvider.Setup(m => m.GetService(typeof(IServiceScopeFactory))).Returns(serviceScopeFactory.Object);
        serviceScopeFactory.Setup(m => m.CreateScope()).Returns(serviceScope.Object);
        serviceScope.Setup(m => m.ServiceProvider).Returns(serviceProvider.Object);

        var repository = new Mock<IIntegrationEventRepository>();
        serviceProvider.Setup(m => m.GetService(typeof(IIntegrationEventRepository))).Returns(repository.Object);
        repository.Setup(m => m.FindAsync(transactionId, default)).ReturnsAsync(entries);

        var eventPublisher = new Mock<IEventPublisher>();
        serviceProvider.Setup(m => m.GetService(typeof(IEventPublisher))).Returns(eventPublisher.Object);

        var logger = new Mock<ILogger<OutboxWorker>>();

        var outboxWorker = new OutboxWorker(repository.Object, eventPublisher.Object, logger.Object);

        // When
        await outboxWorker.ProcessAsync(transactionId, default);

        // Then
        repository.Verify(m => m.FindAsync(transactionId, default), Times.Once());
        repository.Verify(m => m.SaveAsync(entries, default), Times.Exactly(2));
        eventPublisher.Verify(m => m.PublishAsync(It.Is<IIntegrationEvent>(e => e is SomethingIntegrationEvent && e.Id == entry.Id), default), Times.Once());
    }

    [Fact]
    public void 如果FindAsync沒有找到任何IntegrationEventEntity_應該不做事()
    {
        // Given
        var entry = GetIntegrationEventEntity();
        var transactionId = entry.TransactionId;

        var repository = new Mock<IIntegrationEventRepository>();
        repository.Setup(m => m.FindAsync(transactionId, default)).ReturnsAsync(new List<IntegrationEventEntity>());

        var eventPublisher = new Mock<IEventPublisher>();

        var logger = new Mock<ILogger<OutboxWorker>>();

        var outboxWorker = new OutboxWorker(repository.Object, eventPublisher.Object, logger.Object);

        // When

        // Then
        repository.Verify(m => m.SaveAsync(It.IsAny<IEnumerable<IntegrationEventEntity>>(), default), Times.Never());
        eventPublisher.Verify(m => m.PublishAsync(It.Is<IIntegrationEvent>(e => e is SomethingIntegrationEvent && e.Id == entry.Id), default), Times.Never());
    }

    [Fact]
    public async Task 如果FindAsync拋出例外_應該被記錄()
    {
        // Given
        var entry = GetIntegrationEventEntity();
        var entries = new List<IntegrationEventEntity> { entry };
        var transactionId = entry.TransactionId;

        var repository = new Mock<IIntegrationEventRepository>();
        repository.Setup(m => m.FindAsync(It.Is<Guid>(id => id == transactionId), default)).ThrowsAsync(new Exception());

        var eventPublisher = new Mock<IEventPublisher>();

        var logger = new Mock<ILogger<OutboxWorker>>();

        var outboxWorker = new OutboxWorker(repository.Object, eventPublisher.Object, logger.Object);

        // When
        var func = () => outboxWorker.ProcessAsync(transactionId, default);

        // Then
        await func.Should().ThrowAsync<Exception>();
        logger.Verify(logger => logger.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception?>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task 如果第一次SaveAsync拋出例外_應該被記錄()
    {
        // Given
        var entry = GetIntegrationEventEntity();
        var entries = new List<IntegrationEventEntity> { entry };
        var transactionId = entry.TransactionId;

        var repository = new Mock<IIntegrationEventRepository>();
        repository.Setup(m => m.FindAsync(transactionId, default)).ReturnsAsync(entries);
        repository.Setup(m => m.SaveAsync(entries, default)).ThrowsAsync(new Exception());

        var eventPublisher = new Mock<IEventPublisher>();

        var logger = new Mock<ILogger<OutboxWorker>>();

        var outboxWorker = new OutboxWorker(repository.Object, eventPublisher.Object, logger.Object);

        // When
        var func = () => outboxWorker.ProcessAsync(transactionId, default);

        // Then
        await func.Should().ThrowAsync<Exception>();
        repository.Verify(m => m.SaveAsync(entries, default), Times.Once());
        logger.Verify(logger => logger.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception?>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task 如果執行發佈Events的動作拋出例外_應該被記錄()
    {
        // Given
        var entry = GetIntegrationEventEntity();
        var entries = new List<IntegrationEventEntity> { entry };
        var payloads = entries.Select(e => e.GetPayload()).ToList();
        var transactionId = entry.TransactionId;

        var repository = new Mock<IIntegrationEventRepository>();
        repository.Setup(m => m.FindAsync(transactionId, default)).ReturnsAsync(entries);

        var eventPublisher = new Mock<IEventPublisher>();
        eventPublisher.Setup(m => m.PublishAsync(It.IsAny<IIntegrationEvent>(), default)).ThrowsAsync(new Exception());

        var logger = new Mock<ILogger<OutboxWorker>>();

        var outboxWorker = new OutboxWorker(repository.Object, eventPublisher.Object, logger.Object);

        // When
        var func = () => outboxWorker.ProcessAsync(transactionId, default);

        // Then
        await func.Should().ThrowAsync<Exception>();
        eventPublisher.Verify(m => m.PublishAsync(It.Is<IIntegrationEvent>(e => e is SomethingIntegrationEvent && e.Id == entry.Id), default), Times.Once());
        logger.Verify(logger => logger.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception?>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task 如果第二次SaveAsync拋出例外_應該被記錄()
    {
        // Given
        var entry = GetIntegrationEventEntity();
        var entries = new List<IntegrationEventEntity> { entry };
        var transactionId = entry.TransactionId;

        var repository = new Mock<IIntegrationEventRepository>();
        repository.Setup(m => m.FindAsync(transactionId, default)).ReturnsAsync(entries);
        repository.SetupSequence(m => m.SaveAsync(entries, default))
            .Returns(Task.CompletedTask)
            .ThrowsAsync(new Exception());

        var eventPublisher = new Mock<IEventPublisher>();

        var logger = new Mock<ILogger<OutboxWorker>>();

        var outboxWorker = new OutboxWorker(repository.Object, eventPublisher.Object, logger.Object);

        // When
        var func = () => outboxWorker.ProcessAsync(transactionId, default);

        // Then
        await func.Should().ThrowAsync<Exception>();
        repository.Verify(m => m.SaveAsync(entries, default), Times.Exactly(2));
        logger.Verify(logger => logger.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception?>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    private static Payload GetPayload()
    {
        var somethingIntegrationEvent = new SomethingIntegrationEvent();
        var payload = Payload.Serialize(somethingIntegrationEvent);
        return payload;
    }

    private static IntegrationEventEntity GetIntegrationEventEntity()
    {
        var payload = GetPayload();
        var transactionId = Guid.NewGuid();
        var entry = IntegrationEventEntity.Raise(payload, transactionId);
        return entry;
    }
}
