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

        serviceProvider.Setup(m => m.GetService(type)).Returns(handler.Object);

        var consumer = new EventConsumer(serviceProvider.Object, logger.Object);
        var integrationEvent = new SomethingIntegrationEvent();

        // When
        await consumer.ConsumeAsync(integrationEvent, default);

        // Then
        serviceProvider.Verify(m => m.GetService(type), Times.Once());
        handler.Verify(m => m.HandleAsync(integrationEvent, default), Times.Once());
    }
}
