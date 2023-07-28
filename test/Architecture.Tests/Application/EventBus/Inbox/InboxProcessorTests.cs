using Architecture.Application.EventBus.Inbox;
using Architecture.Domain.EventBus;
using Architecture.Domain.EventBus.Inbox;
using CSharpFunctionalExtensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Architecture.Tests.Application.EventBus.Inbox;

public class InboxProcessorTests
{
    [Fact]
    public async Task 應該會選取IntegrationEventEntryId對應的Event操作Progress_接著執行處理Event的Func_最後操作Consume()
    {
        // Given
        var entry = GetIntegrationEventEntry();
        var integrationEventEntryId = entry.Id;
        var payload = entry.GetPayload();

        var serviceProvider = new Mock<IServiceProvider>();
        var serviceScopeFactory = new Mock<IServiceScopeFactory>();
        var serviceScope = new Mock<IServiceScope>();

        serviceProvider.Setup(m => m.GetService(typeof(IServiceScopeFactory))).Returns(serviceScopeFactory.Object);
        serviceScopeFactory.Setup(m => m.CreateScope()).Returns(serviceScope.Object);
        serviceScope.Setup(m => m.ServiceProvider).Returns(serviceProvider.Object);

        var repository = new Mock<IIntegrationEventRepository>();
        serviceProvider.Setup(m => m.GetService(typeof(IIntegrationEventRepository))).Returns(repository.Object);
        repository.Setup(m => m.FindAsync(integrationEventEntryId, default)).ReturnsAsync(entry);

        var logger = new Mock<ILogger<InboxProcessor>>();
        var func = new Mock<Func<IServiceProvider, Payload, Task>>();

        var inboxProcessor = new InboxProcessor(serviceProvider.Object, logger.Object, func.Object);

        // When
        await inboxProcessor.ProcessAsync(integrationEventEntryId, default);

        // Then
        repository.Verify(m => m.FindAsync(integrationEventEntryId, default), Times.Once());
        repository.Verify(m => m.SaveAsync(entry, default), Times.Exactly(2));
        func.Verify(m => m(serviceProvider.Object, payload), Times.Once());
    }

    [Fact]
    public async Task 如果FindAsync沒有找到IntegrationEventEntry_應該不做事()
    {
        // Given
        var entry = GetIntegrationEventEntry();
        var integrationEventEntryId = entry.Id;
        var payload = entry.GetPayload();


        var serviceProvider = new Mock<IServiceProvider>();
        var serviceScopeFactory = new Mock<IServiceScopeFactory>();
        var serviceScope = new Mock<IServiceScope>();

        serviceProvider.Setup(m => m.GetService(typeof(IServiceScopeFactory))).Returns(serviceScopeFactory.Object);
        serviceScopeFactory.Setup(m => m.CreateScope()).Returns(serviceScope.Object);
        serviceScope.Setup(m => m.ServiceProvider).Returns(serviceProvider.Object);

        var repository = new Mock<IIntegrationEventRepository>();
        serviceProvider.Setup(m => m.GetService(typeof(IIntegrationEventRepository))).Returns(repository.Object);
        repository.Setup(m => m.FindAsync(integrationEventEntryId, default)).ReturnsAsync(Maybe<IntegrationEventEntry>.None);

        var logger = new Mock<ILogger<InboxProcessor>>();
        var func = new Mock<Func<IServiceProvider, Payload, Task>>();

        var inboxProcessor = new InboxProcessor(serviceProvider.Object, logger.Object, func.Object);

        // When
        await inboxProcessor.ProcessAsync(integrationEventEntryId, default);

        // Then
        repository.Verify(m => m.SaveAsync(entry, default), Times.Never());
        func.Verify(m => m(serviceProvider.Object, payload), Times.Never());
    }

    [Fact]
    public async Task 如果FindAsync拋出例外_應該被記錄()
    {
        // Given
        var entry = GetIntegrationEventEntry();
        var integrationEventEntryId = entry.Id;

        var serviceProvider = new Mock<IServiceProvider>();
        var serviceScopeFactory = new Mock<IServiceScopeFactory>();
        var serviceScope = new Mock<IServiceScope>();

        serviceProvider.Setup(m => m.GetService(typeof(IServiceScopeFactory))).Returns(serviceScopeFactory.Object);
        serviceScopeFactory.Setup(m => m.CreateScope()).Returns(serviceScope.Object);
        serviceScope.Setup(m => m.ServiceProvider).Returns(serviceProvider.Object);

        var repository = new Mock<IIntegrationEventRepository>();
        serviceProvider.Setup(m => m.GetService(typeof(IIntegrationEventRepository))).Returns(repository.Object);
        repository.Setup(m => m.FindAsync(integrationEventEntryId, default)).ThrowsAsync(new Exception());

        var logger = new Mock<ILogger<InboxProcessor>>();
        var func = new Mock<Func<IServiceProvider, Payload, Task>>();

        var inboxProcessor = new InboxProcessor(serviceProvider.Object, logger.Object, func.Object);

        // When
        await inboxProcessor.ProcessAsync(integrationEventEntryId, default);

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
        var entry = GetIntegrationEventEntry();
        var integrationEventEntryId = entry.Id;

        var serviceProvider = new Mock<IServiceProvider>();
        var serviceScopeFactory = new Mock<IServiceScopeFactory>();
        var serviceScope = new Mock<IServiceScope>();

        serviceProvider.Setup(m => m.GetService(typeof(IServiceScopeFactory))).Returns(serviceScopeFactory.Object);
        serviceScopeFactory.Setup(m => m.CreateScope()).Returns(serviceScope.Object);
        serviceScope.Setup(m => m.ServiceProvider).Returns(serviceProvider.Object);

        var repository = new Mock<IIntegrationEventRepository>();
        serviceProvider.Setup(m => m.GetService(typeof(IIntegrationEventRepository))).Returns(repository.Object);
        repository.Setup(m => m.FindAsync(integrationEventEntryId, default)).ReturnsAsync(entry);
        repository.Setup(m => m.SaveAsync(entry, default)).ThrowsAsync(new Exception());

        var logger = new Mock<ILogger<InboxProcessor>>();
        var func = new Mock<Func<IServiceProvider, Payload, Task>>();

        var inboxProcessor = new InboxProcessor(serviceProvider.Object, logger.Object, func.Object);

        // When
        await inboxProcessor.ProcessAsync(integrationEventEntryId, default);

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
    public async Task 如果執行處理Event的Func拋出例外_應該被記錄()
    {
        // Given
        var entry = GetIntegrationEventEntry();
        var integrationEventEntryId = entry.Id;
        var payload = entry.GetPayload();

        var serviceProvider = new Mock<IServiceProvider>();
        var serviceScopeFactory = new Mock<IServiceScopeFactory>();
        var serviceScope = new Mock<IServiceScope>();

        serviceProvider.Setup(m => m.GetService(typeof(IServiceScopeFactory))).Returns(serviceScopeFactory.Object);
        serviceScopeFactory.Setup(m => m.CreateScope()).Returns(serviceScope.Object);
        serviceScope.Setup(m => m.ServiceProvider).Returns(serviceProvider.Object);

        var repository = new Mock<IIntegrationEventRepository>();
        serviceProvider.Setup(m => m.GetService(typeof(IIntegrationEventRepository))).Returns(repository.Object);
        repository.Setup(m => m.FindAsync(integrationEventEntryId, default)).ReturnsAsync(entry);

        var logger = new Mock<ILogger<InboxProcessor>>();
        var func = new Mock<Func<IServiceProvider, Payload, Task>>();
        func.Setup(m => m(serviceProvider.Object, payload)).ThrowsAsync(new Exception());

        var inboxProcessor = new InboxProcessor(serviceProvider.Object, logger.Object, func.Object);

        // When
        await inboxProcessor.ProcessAsync(integrationEventEntryId, default);

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
        var entry = GetIntegrationEventEntry();
        var integrationEventEntryId = entry.Id;

        var serviceProvider = new Mock<IServiceProvider>();
        var serviceScopeFactory = new Mock<IServiceScopeFactory>();
        var serviceScope = new Mock<IServiceScope>();

        serviceProvider.Setup(m => m.GetService(typeof(IServiceScopeFactory))).Returns(serviceScopeFactory.Object);
        serviceScopeFactory.Setup(m => m.CreateScope()).Returns(serviceScope.Object);
        serviceScope.Setup(m => m.ServiceProvider).Returns(serviceProvider.Object);

        var repository = new Mock<IIntegrationEventRepository>();
        serviceProvider.Setup(m => m.GetService(typeof(IIntegrationEventRepository))).Returns(repository.Object);
        repository.Setup(m => m.FindAsync(integrationEventEntryId, default)).ReturnsAsync(entry);
        repository.SetupSequence(m => m.SaveAsync(entry, default))
            .Returns(Task.CompletedTask)
            .ThrowsAsync(new Exception());

        var logger = new Mock<ILogger<InboxProcessor>>();
        var func = new Mock<Func<IServiceProvider, Payload, Task>>();

        var inboxProcessor = new InboxProcessor(serviceProvider.Object, logger.Object, func.Object);

        // When
        await inboxProcessor.ProcessAsync(integrationEventEntryId, default);

        // Then
        repository.Verify(m => m.SaveAsync(entry, default), Times.Exactly(2));
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

    private static IntegrationEventEntry GetIntegrationEventEntry()
    {
        var payload = GetPayload();
        var entry = IntegrationEventEntry.Receive(payload);
        return entry;
    }
}
