using Architecture.Shell.EventBus.Inbox;
using Architecture.Shell.EventBus;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace Architecture.Tests.Shell.EventBus.Inbox;

public class EventConsumerTests
{
    [Fact]
    public async Task 應該能夠取得IntegrationEventHandlers並進行處理()
    {
        // Given
        var logger = new Mock<ILogger<EventConsumer>>();
        var handler = new Mock<IIntegrationEventHandler<SomethingIntegrationEvent>>();
        var type = typeof(IIntegrationEventHandler<>).MakeGenericType(typeof(SomethingIntegrationEvent));

        var serviceProvider = new Mock<IServiceProvider>();
        var serviceScopeFactory = new Mock<IServiceScopeFactory>();
        var serviceScope = new Mock<IServiceScope>();

        serviceScopeFactory.Setup(m => m.CreateScope()).Returns(serviceScope.Object);
        serviceScope.Setup(m => m.ServiceProvider).Returns(serviceProvider.Object);
        serviceProvider.Setup(m => m.GetService(type)).Returns(handler.Object);

        var consumer = new EventConsumer(serviceScopeFactory.Object, logger.Object);
        var integrationEvent = new SomethingIntegrationEvent();

        // When
        await consumer.ConsumeAsync(integrationEvent, default);

        // Then
        serviceScopeFactory.Verify(m => m.CreateScope(), Times.Once());
        serviceScope.Verify(m => m.ServiceProvider, Times.Once());
        serviceProvider.Verify(m => m.GetService(type), Times.Once());
        handler.Verify(m => m.HandleAsync(integrationEvent, default), Times.Once());
    }
}
