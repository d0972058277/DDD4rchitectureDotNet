using Architecture.Application.EventBus.Inbox;
using Architecture.Domain.EventBus;
using Architecture.Domain.EventBus.Inbox;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Architecture.Tests.Application.EventBus.Inbox;

public class InboxProcessorTests
{
    [Fact]
    public async Task 應該會選取IntegrationEventEntryId對應的Event操作Progress_接著執行處理Event的Func_最後操作Consume()
    {
        // Given
        var integrationEventEntry = GetIntegrationEventEntry();
        var integrationEvent = integrationEventEntry.GetIntegrationEvent();
        var integrationEventId = integrationEventEntry.Id;

        var serviceProvider = new Mock<IServiceProvider>();
        var serviceScopeFactory = new Mock<IServiceScopeFactory>();
        var serviceScope = new Mock<IServiceScope>();

        serviceProvider.Setup(m => m.GetService(typeof(IServiceScopeFactory))).Returns(serviceScopeFactory.Object);
        serviceScopeFactory.Setup(m => m.CreateScope()).Returns(serviceScope.Object);
        serviceScope.Setup(m => m.ServiceProvider).Returns(serviceProvider.Object);

        var repository = new Mock<IIntegrationEventRepository>();
        serviceProvider.Setup(m => m.GetService(typeof(IIntegrationEventRepository))).Returns(repository.Object);
        repository.Setup(m => m.FindAsync(integrationEventId, default)).ReturnsAsync(integrationEventEntry);

        var logger = new Mock<ILogger<InboxProcessor>>();
        var func = new Mock<Func<IServiceProvider, IntegrationEvent, Task>>();

        var inboxProcessor = new InboxProcessor(serviceProvider.Object, logger.Object, func.Object);

        // When
        await inboxProcessor.ProcessAsync(integrationEventId, default);

        // Then
        repository.Verify(m => m.FindAsync(integrationEventId, default), Times.Once());
        repository.Verify(m => m.SaveAsync(integrationEventEntry, default), Times.Exactly(2));
        func.Verify(m => m(serviceProvider.Object, integrationEvent), Times.Once());
    }

    [Fact]
    public async Task 如果FindAsync拋出例外_應該被記錄()
    {
        // Given
        var integrationEventEntry = GetIntegrationEventEntry();
        var integrationEvent = integrationEventEntry.GetIntegrationEvent();
        var integrationEventId = integrationEventEntry.Id;

        var serviceProvider = new Mock<IServiceProvider>();
        var serviceScopeFactory = new Mock<IServiceScopeFactory>();
        var serviceScope = new Mock<IServiceScope>();

        serviceProvider.Setup(m => m.GetService(typeof(IServiceScopeFactory))).Returns(serviceScopeFactory.Object);
        serviceScopeFactory.Setup(m => m.CreateScope()).Returns(serviceScope.Object);
        serviceScope.Setup(m => m.ServiceProvider).Returns(serviceProvider.Object);

        var repository = new Mock<IIntegrationEventRepository>();
        serviceProvider.Setup(m => m.GetService(typeof(IIntegrationEventRepository))).Returns(repository.Object);
        repository.Setup(m => m.FindAsync(integrationEventId, default)).ThrowsAsync(new Exception());

        var logger = new Mock<ILogger<InboxProcessor>>();
        var func = new Mock<Func<IServiceProvider, IntegrationEvent, Task>>();

        var inboxProcessor = new InboxProcessor(serviceProvider.Object, logger.Object, func.Object);

        // When
        await inboxProcessor.ProcessAsync(integrationEventId, default);

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
        var integrationEventEntry = GetIntegrationEventEntry();
        var integrationEvent = integrationEventEntry.GetIntegrationEvent();
        var integrationEventId = integrationEventEntry.Id;

        var serviceProvider = new Mock<IServiceProvider>();
        var serviceScopeFactory = new Mock<IServiceScopeFactory>();
        var serviceScope = new Mock<IServiceScope>();

        serviceProvider.Setup(m => m.GetService(typeof(IServiceScopeFactory))).Returns(serviceScopeFactory.Object);
        serviceScopeFactory.Setup(m => m.CreateScope()).Returns(serviceScope.Object);
        serviceScope.Setup(m => m.ServiceProvider).Returns(serviceProvider.Object);

        var repository = new Mock<IIntegrationEventRepository>();
        serviceProvider.Setup(m => m.GetService(typeof(IIntegrationEventRepository))).Returns(repository.Object);
        repository.Setup(m => m.FindAsync(integrationEventId, default)).ReturnsAsync(integrationEventEntry);
        repository.Setup(m => m.SaveAsync(integrationEventEntry, default)).ThrowsAsync(new Exception());

        var logger = new Mock<ILogger<InboxProcessor>>();
        var func = new Mock<Func<IServiceProvider, IntegrationEvent, Task>>();

        var inboxProcessor = new InboxProcessor(serviceProvider.Object, logger.Object, func.Object);

        // When
        await inboxProcessor.ProcessAsync(integrationEventId, default);

        // Then
        repository.Verify(m => m.SaveAsync(integrationEventEntry, default), Times.Once());
        logger.Verify(logger => logger.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception?>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task 如果執行處理Event的Func拋出例外_應該被記錄()
    {
        // Given
        var integrationEventEntry = GetIntegrationEventEntry();
        var integrationEvent = integrationEventEntry.GetIntegrationEvent();
        var integrationEventId = integrationEventEntry.Id;

        var serviceProvider = new Mock<IServiceProvider>();
        var serviceScopeFactory = new Mock<IServiceScopeFactory>();
        var serviceScope = new Mock<IServiceScope>();

        serviceProvider.Setup(m => m.GetService(typeof(IServiceScopeFactory))).Returns(serviceScopeFactory.Object);
        serviceScopeFactory.Setup(m => m.CreateScope()).Returns(serviceScope.Object);
        serviceScope.Setup(m => m.ServiceProvider).Returns(serviceProvider.Object);

        var repository = new Mock<IIntegrationEventRepository>();
        serviceProvider.Setup(m => m.GetService(typeof(IIntegrationEventRepository))).Returns(repository.Object);
        repository.Setup(m => m.FindAsync(integrationEventId, default)).ReturnsAsync(integrationEventEntry);

        var logger = new Mock<ILogger<InboxProcessor>>();
        var func = new Mock<Func<IServiceProvider, IntegrationEvent, Task>>();
        func.Setup(m => m(serviceProvider.Object, integrationEvent)).ThrowsAsync(new Exception());

        var inboxProcessor = new InboxProcessor(serviceProvider.Object, logger.Object, func.Object);

        // When
        await inboxProcessor.ProcessAsync(integrationEventId, default);

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
    public async Task 如果第二次SaveAsync拋出例外_應該被記錄()
    {
        // Given
        var integrationEventEntry = GetIntegrationEventEntry();
        var integrationEvent = integrationEventEntry.GetIntegrationEvent();
        var integrationEventId = integrationEventEntry.Id;

        var serviceProvider = new Mock<IServiceProvider>();
        var serviceScopeFactory = new Mock<IServiceScopeFactory>();
        var serviceScope = new Mock<IServiceScope>();

        serviceProvider.Setup(m => m.GetService(typeof(IServiceScopeFactory))).Returns(serviceScopeFactory.Object);
        serviceScopeFactory.Setup(m => m.CreateScope()).Returns(serviceScope.Object);
        serviceScope.Setup(m => m.ServiceProvider).Returns(serviceProvider.Object);

        var repository = new Mock<IIntegrationEventRepository>();
        serviceProvider.Setup(m => m.GetService(typeof(IIntegrationEventRepository))).Returns(repository.Object);
        repository.Setup(m => m.FindAsync(integrationEventId, default)).ReturnsAsync(integrationEventEntry);
        repository.SetupSequence(m => m.SaveAsync(integrationEventEntry, default))
            .Returns(Task.CompletedTask)
            .ThrowsAsync(new Exception());

        var logger = new Mock<ILogger<InboxProcessor>>();
        var func = new Mock<Func<IServiceProvider, IntegrationEvent, Task>>();

        var inboxProcessor = new InboxProcessor(serviceProvider.Object, logger.Object, func.Object);

        // When
        await inboxProcessor.ProcessAsync(integrationEventId, default);

        // Then
        repository.Verify(m => m.SaveAsync(integrationEventEntry, default), Times.Exactly(2));
        logger.Verify(logger => logger.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception?>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    private static IntegrationEvent GetIntegrationEvent()
    {
        var somethingIntegrationEvent = new SomethingIntegrationEvent();
        var integrationEvent = IntegrationEvent.Create(somethingIntegrationEvent);
        return integrationEvent;
    }

    private static IntegrationEventEntry GetIntegrationEventEntry()
    {
        var integrationEvent = GetIntegrationEvent();
        var integrationEventEntry = IntegrationEventEntry.Receive(integrationEvent);
        return integrationEventEntry;
    }
}
