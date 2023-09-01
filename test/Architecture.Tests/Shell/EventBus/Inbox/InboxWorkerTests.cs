using Architecture.Shell.EventBus;
using Architecture.Shell.EventBus.Inbox;
using CSharpFunctionalExtensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Architecture.Tests.Shell.EventBus.Inbox;

public class InboxWorkerTests
{
    [Fact]
    public async Task 應該會選取IntegrationEventEntityId對應的Event操作Progress_接著執行處理Event的Func_最後操作Consume()
    {
        // Given
        var entry = GetIntegrationEventEntity();
        var integrationEventEntityId = entry.Id;
        var payload = entry.GetPayload();
        var integrationEvent = payload.Deserialize<SomethingIntegrationEvent>();

        var serviceProvider = new Mock<IServiceProvider>();
        var serviceScopeFactory = new Mock<IServiceScopeFactory>();
        var serviceScope = new Mock<IServiceScope>();

        serviceProvider.Setup(m => m.GetService(typeof(IServiceScopeFactory))).Returns(serviceScopeFactory.Object);
        serviceScopeFactory.Setup(m => m.CreateScope()).Returns(serviceScope.Object);
        serviceScope.Setup(m => m.ServiceProvider).Returns(serviceProvider.Object);

        var repository = new Mock<IIntegrationEventRepository>();
        serviceProvider.Setup(m => m.GetService(typeof(IIntegrationEventRepository))).Returns(repository.Object);
        repository.Setup(m => m.FindAsync(integrationEventEntityId, default)).ReturnsAsync(entry);

        var eventConsumer = new Mock<IEventConsumer>();
        serviceProvider.Setup(m => m.GetService(typeof(IEventConsumer))).Returns(eventConsumer.Object);

        var logger = new Mock<ILogger<InboxWorker>>();

        var inboxWorker = new InboxWorker(serviceProvider.Object, logger.Object);

        // When
        await inboxWorker.ProcessAsync<SomethingIntegrationEvent>(integrationEvent, default);

        // Then
        repository.Verify(m => m.FindAsync(integrationEventEntityId, default), Times.Once());
        repository.Verify(m => m.SaveAsync(entry, default), Times.Once());
        eventConsumer.Verify(m => m.ConsumeAsync(It.Is<IIntegrationEvent>(e => e is SomethingIntegrationEvent && e.Id == entry.Id), default), Times.Once());
    }

    [Fact]
    public async Task 如果FindAsync沒有找到IntegrationEventEntity_應該不做事()
    {
        // Given
        var entry = GetIntegrationEventEntity();
        var integrationEventEntityId = entry.Id;
        var payload = entry.GetPayload();
        var integrationEvent = payload.Deserialize<SomethingIntegrationEvent>();

        var serviceProvider = new Mock<IServiceProvider>();
        var serviceScopeFactory = new Mock<IServiceScopeFactory>();
        var serviceScope = new Mock<IServiceScope>();

        serviceProvider.Setup(m => m.GetService(typeof(IServiceScopeFactory))).Returns(serviceScopeFactory.Object);
        serviceScopeFactory.Setup(m => m.CreateScope()).Returns(serviceScope.Object);
        serviceScope.Setup(m => m.ServiceProvider).Returns(serviceProvider.Object);

        var repository = new Mock<IIntegrationEventRepository>();
        serviceProvider.Setup(m => m.GetService(typeof(IIntegrationEventRepository))).Returns(repository.Object);
        repository.Setup(m => m.FindAsync(integrationEventEntityId, default)).ReturnsAsync(Maybe<IntegrationEventEntity>.None);

        var eventConsumer = new Mock<IEventConsumer>();
        serviceProvider.Setup(m => m.GetService(typeof(IEventConsumer))).Returns(eventConsumer.Object);

        var logger = new Mock<ILogger<InboxWorker>>();

        var inboxWorker = new InboxWorker(serviceProvider.Object, logger.Object);

        // When
        await inboxWorker.ProcessAsync<SomethingIntegrationEvent>(integrationEvent, default);

        // Then
        repository.Verify(m => m.SaveAsync(entry, default), Times.Never());
        eventConsumer.Verify(m => m.ConsumeAsync(It.Is<IIntegrationEvent>(e => e is SomethingIntegrationEvent && e.Id == entry.Id), default), Times.Never());
    }

    [Fact]
    public async Task 如果FindAsync拋出例外_應該被記錄()
    {
        // Given
        var entry = GetIntegrationEventEntity();
        var integrationEventEntityId = entry.Id;
        var payload = entry.GetPayload();
        var integrationEvent = payload.Deserialize<SomethingIntegrationEvent>();

        var serviceProvider = new Mock<IServiceProvider>();
        var serviceScopeFactory = new Mock<IServiceScopeFactory>();
        var serviceScope = new Mock<IServiceScope>();

        serviceProvider.Setup(m => m.GetService(typeof(IServiceScopeFactory))).Returns(serviceScopeFactory.Object);
        serviceScopeFactory.Setup(m => m.CreateScope()).Returns(serviceScope.Object);
        serviceScope.Setup(m => m.ServiceProvider).Returns(serviceProvider.Object);

        var repository = new Mock<IIntegrationEventRepository>();
        serviceProvider.Setup(m => m.GetService(typeof(IIntegrationEventRepository))).Returns(repository.Object);
        repository.Setup(m => m.FindAsync(integrationEventEntityId, default)).ThrowsAsync(new Exception());

        var eventConsumer = new Mock<IEventConsumer>();
        serviceProvider.Setup(m => m.GetService(typeof(IEventConsumer))).Returns(eventConsumer.Object);

        var logger = new Mock<ILogger<InboxWorker>>();

        var inboxWorker = new InboxWorker(serviceProvider.Object, logger.Object);

        // When
        await inboxWorker.ProcessAsync<SomethingIntegrationEvent>(integrationEvent, default);

        // Then
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
        var integrationEventEntityId = entry.Id;
        var payload = entry.GetPayload();
        var integrationEvent = payload.Deserialize<SomethingIntegrationEvent>();

        var serviceProvider = new Mock<IServiceProvider>();
        var serviceScopeFactory = new Mock<IServiceScopeFactory>();
        var serviceScope = new Mock<IServiceScope>();

        serviceProvider.Setup(m => m.GetService(typeof(IServiceScopeFactory))).Returns(serviceScopeFactory.Object);
        serviceScopeFactory.Setup(m => m.CreateScope()).Returns(serviceScope.Object);
        serviceScope.Setup(m => m.ServiceProvider).Returns(serviceProvider.Object);

        var repository = new Mock<IIntegrationEventRepository>();
        serviceProvider.Setup(m => m.GetService(typeof(IIntegrationEventRepository))).Returns(repository.Object);
        repository.Setup(m => m.FindAsync(integrationEventEntityId, default)).ReturnsAsync(entry);
        repository.Setup(m => m.SaveAsync(entry, default)).ThrowsAsync(new Exception());

        var eventConsumer = new Mock<IEventConsumer>();
        serviceProvider.Setup(m => m.GetService(typeof(IEventConsumer))).Returns(eventConsumer.Object);

        var logger = new Mock<ILogger<InboxWorker>>();

        var inboxWorker = new InboxWorker(serviceProvider.Object, logger.Object);

        // When
        await inboxWorker.ProcessAsync<SomethingIntegrationEvent>(integrationEvent, default);

        // Then
        repository.Verify(m => m.SaveAsync(entry, default), Times.Once());
        logger.Verify(logger => logger.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception?>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task 如果執行處理Event的動作拋出例外_應該被記錄()
    {
        // Given
        var entry = GetIntegrationEventEntity();
        var integrationEventEntityId = entry.Id;
        var payload = entry.GetPayload();
        var integrationEvent = payload.Deserialize<SomethingIntegrationEvent>();

        var serviceProvider = new Mock<IServiceProvider>();
        var serviceScopeFactory = new Mock<IServiceScopeFactory>();
        var serviceScope = new Mock<IServiceScope>();

        serviceProvider.Setup(m => m.GetService(typeof(IServiceScopeFactory))).Returns(serviceScopeFactory.Object);
        serviceScopeFactory.Setup(m => m.CreateScope()).Returns(serviceScope.Object);
        serviceScope.Setup(m => m.ServiceProvider).Returns(serviceProvider.Object);

        var repository = new Mock<IIntegrationEventRepository>();
        serviceProvider.Setup(m => m.GetService(typeof(IIntegrationEventRepository))).Returns(repository.Object);
        repository.Setup(m => m.FindAsync(integrationEventEntityId, default)).ReturnsAsync(entry);

        var eventConsumer = new Mock<IEventConsumer>();
        eventConsumer.Setup(m => m.ConsumeAsync(It.IsAny<IIntegrationEvent>(), default)).ThrowsAsync(new Exception());
        serviceProvider.Setup(m => m.GetService(typeof(IEventConsumer))).Returns(eventConsumer.Object);

        var logger = new Mock<ILogger<InboxWorker>>();

        var inboxWorker = new InboxWorker(serviceProvider.Object, logger.Object);

        // When
        await inboxWorker.ProcessAsync<SomethingIntegrationEvent>(integrationEvent, default);

        // Then
        eventConsumer.Verify(m => m.ConsumeAsync(It.Is<IIntegrationEvent>(e => e is SomethingIntegrationEvent && e.Id == entry.Id), default), Times.Once());
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
        var entry = IntegrationEventEntity.Receive(payload);
        return entry;
    }
}
