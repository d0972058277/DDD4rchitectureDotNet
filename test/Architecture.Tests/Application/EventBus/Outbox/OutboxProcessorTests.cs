using Architecture.Application.EventBus.Outbox;
using Architecture.Domain.EventBus;
using Architecture.Domain.EventBus.Outbox;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Architecture.Tests.Application.EventBus.Outbox;

public class OutboxProcessorTests
{
    [Fact]
    public async Task 應該會選取TransactionId對應的Events操作Progress_接著執行發佈Events的Func_最後操作Publish()
    {
        // Given
        var integrationEventEntry = GetIntegrationEventEntry();
        var integrationEventEntries = new List<IntegrationEventEntry> { integrationEventEntry };
        var integrationEvents = integrationEventEntries.Select(e => e.GetIntegrationEvent()).ToList();
        var transactionId = integrationEventEntry.TransactionId;

        var serviceProvider = new Mock<IServiceProvider>();
        var serviceScopeFactory = new Mock<IServiceScopeFactory>();
        var serviceScope = new Mock<IServiceScope>();

        serviceProvider.Setup(m => m.GetService(typeof(IServiceScopeFactory))).Returns(serviceScopeFactory.Object);
        serviceScopeFactory.Setup(m => m.CreateScope()).Returns(serviceScope.Object);
        serviceScope.Setup(m => m.ServiceProvider).Returns(serviceProvider.Object);

        var repository = new Mock<IIntegrationEventRepository>();
        serviceProvider.Setup(m => m.GetService(typeof(IIntegrationEventRepository))).Returns(repository.Object);
        repository.Setup(m => m.FindAsync(transactionId, default)).ReturnsAsync(integrationEventEntries);

        var logger = new Mock<ILogger<OutboxProcessor>>();
        var func = new Mock<Func<IServiceProvider, IEnumerable<IntegrationEvent>, Task>>();

        var outboxProcessor = new OutboxProcessor(serviceProvider.Object, logger.Object, func.Object);

        // When
        await outboxProcessor.ProcessAsync(transactionId, default);

        // Then
        repository.Verify(m => m.FindAsync(transactionId, default), Times.Once());
        repository.Verify(m => m.SaveAsync(integrationEventEntries, default), Times.Exactly(2));
        func.Verify(m => m(serviceProvider.Object, It.Is<IEnumerable<IntegrationEvent>>(l => l.SequenceEqual(integrationEvents))), Times.Once());
    }

    [Fact]
    public async Task 如果FindAsync拋出例外_應該被記錄()
    {
        // Given
        var integrationEventEntry = GetIntegrationEventEntry();
        var integrationEventEntries = new List<IntegrationEventEntry> { integrationEventEntry };
        var integrationEvents = integrationEventEntries.Select(e => e.GetIntegrationEvent()).ToList();
        var transactionId = integrationEventEntry.TransactionId;

        var serviceProvider = new Mock<IServiceProvider>();
        var serviceScopeFactory = new Mock<IServiceScopeFactory>();
        var serviceScope = new Mock<IServiceScope>();

        serviceProvider.Setup(m => m.GetService(typeof(IServiceScopeFactory))).Returns(serviceScopeFactory.Object);
        serviceScopeFactory.Setup(m => m.CreateScope()).Returns(serviceScope.Object);
        serviceScope.Setup(m => m.ServiceProvider).Returns(serviceProvider.Object);

        var repository = new Mock<IIntegrationEventRepository>();
        serviceProvider.Setup(m => m.GetService(typeof(IIntegrationEventRepository))).Returns(repository.Object);
        repository.Setup(m => m.FindAsync(It.Is<Guid>(id => id == transactionId), default)).ThrowsAsync(new Exception());

        var logger = new Mock<ILogger<OutboxProcessor>>();
        var func = new Mock<Func<IServiceProvider, IEnumerable<IntegrationEvent>, Task>>();

        var outboxProcessor = new OutboxProcessor(serviceProvider.Object, logger.Object, func.Object);

        // When
        await outboxProcessor.ProcessAsync(transactionId, default);

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
        var integrationEventEntries = new List<IntegrationEventEntry> { integrationEventEntry };
        var integrationEvents = integrationEventEntries.Select(e => e.GetIntegrationEvent()).ToList();
        var transactionId = integrationEventEntry.TransactionId;

        var serviceProvider = new Mock<IServiceProvider>();
        var serviceScopeFactory = new Mock<IServiceScopeFactory>();
        var serviceScope = new Mock<IServiceScope>();

        serviceProvider.Setup(m => m.GetService(typeof(IServiceScopeFactory))).Returns(serviceScopeFactory.Object);
        serviceScopeFactory.Setup(m => m.CreateScope()).Returns(serviceScope.Object);
        serviceScope.Setup(m => m.ServiceProvider).Returns(serviceProvider.Object);

        var repository = new Mock<IIntegrationEventRepository>();
        serviceProvider.Setup(m => m.GetService(typeof(IIntegrationEventRepository))).Returns(repository.Object);
        repository.Setup(m => m.FindAsync(transactionId, default)).ReturnsAsync(integrationEventEntries);
        repository.Setup(m => m.SaveAsync(integrationEventEntries, default)).ThrowsAsync(new Exception());

        var logger = new Mock<ILogger<OutboxProcessor>>();
        var func = new Mock<Func<IServiceProvider, IEnumerable<IntegrationEvent>, Task>>();

        var outboxProcessor = new OutboxProcessor(serviceProvider.Object, logger.Object, func.Object);

        // When
        await outboxProcessor.ProcessAsync(transactionId, default);

        // Then
        repository.Verify(m => m.SaveAsync(integrationEventEntries, default), Times.Once());
        logger.Verify(logger => logger.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception?>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task 如果執行發佈Events的Func拋出例外_應該被記錄()
    {
        // Given
        var integrationEventEntry = GetIntegrationEventEntry();
        var integrationEventEntries = new List<IntegrationEventEntry> { integrationEventEntry };
        var integrationEvents = integrationEventEntries.Select(e => e.GetIntegrationEvent()).ToList();
        var transactionId = integrationEventEntry.TransactionId;

        var serviceProvider = new Mock<IServiceProvider>();
        var serviceScopeFactory = new Mock<IServiceScopeFactory>();
        var serviceScope = new Mock<IServiceScope>();

        serviceProvider.Setup(m => m.GetService(typeof(IServiceScopeFactory))).Returns(serviceScopeFactory.Object);
        serviceScopeFactory.Setup(m => m.CreateScope()).Returns(serviceScope.Object);
        serviceScope.Setup(m => m.ServiceProvider).Returns(serviceProvider.Object);

        var repository = new Mock<IIntegrationEventRepository>();
        serviceProvider.Setup(m => m.GetService(typeof(IIntegrationEventRepository))).Returns(repository.Object);
        repository.Setup(m => m.FindAsync(transactionId, default)).ReturnsAsync(integrationEventEntries);

        var logger = new Mock<ILogger<OutboxProcessor>>();
        var func = new Mock<Func<IServiceProvider, IEnumerable<IntegrationEvent>, Task>>();
        func.Setup(m => m(serviceProvider.Object, integrationEvents)).ThrowsAsync(new Exception());

        var outboxProcessor = new OutboxProcessor(serviceProvider.Object, logger.Object, func.Object);

        // When
        await outboxProcessor.ProcessAsync(transactionId, default);

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
        var integrationEventEntries = new List<IntegrationEventEntry> { integrationEventEntry };
        var integrationEvents = integrationEventEntries.Select(e => e.GetIntegrationEvent()).ToList();
        var transactionId = integrationEventEntry.TransactionId;

        var serviceProvider = new Mock<IServiceProvider>();
        var serviceScopeFactory = new Mock<IServiceScopeFactory>();
        var serviceScope = new Mock<IServiceScope>();

        serviceProvider.Setup(m => m.GetService(typeof(IServiceScopeFactory))).Returns(serviceScopeFactory.Object);
        serviceScopeFactory.Setup(m => m.CreateScope()).Returns(serviceScope.Object);
        serviceScope.Setup(m => m.ServiceProvider).Returns(serviceProvider.Object);

        var repository = new Mock<IIntegrationEventRepository>();
        serviceProvider.Setup(m => m.GetService(typeof(IIntegrationEventRepository))).Returns(repository.Object);
        repository.Setup(m => m.FindAsync(transactionId, default)).ReturnsAsync(integrationEventEntries);
        repository.SetupSequence(m => m.SaveAsync(integrationEventEntries, default))
            .Returns(Task.CompletedTask)
            .ThrowsAsync(new Exception());

        var logger = new Mock<ILogger<OutboxProcessor>>();
        var func = new Mock<Func<IServiceProvider, IEnumerable<IntegrationEvent>, Task>>();

        var outboxProcessor = new OutboxProcessor(serviceProvider.Object, logger.Object, func.Object);

        // When
        await outboxProcessor.ProcessAsync(transactionId, default);

        // Then
        repository.Verify(m => m.SaveAsync(integrationEventEntries, default), Times.Exactly(2));
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
        var transactionId = Guid.NewGuid();
        var integrationEventEntry = IntegrationEventEntry.Raise(integrationEvent, transactionId);
        return integrationEventEntry;
    }
}
