using Architecture.Shell.EventBus.Inbox;
using Architecture.Shell.EventBus;
using Microsoft.Extensions.Logging;

namespace Architecture.Tests.Shell.EventBus.Inbox;

public class EventConsumerTests
{
    [Fact]
    public async Task 應該能夠取得IntegrationEventHandlers並進行處理()
    {
        // Given
        var logger = new Mock<ILogger<EventConsumer>>();
        var handler = new Mock<IIntegrationEventHandler<SomethingIntegrationEvent>>();
        var handlers = new List<IIntegrationEventHandler<SomethingIntegrationEvent>> { handler.Object };

        var serviceProvider = new Mock<IServiceProvider>();
        serviceProvider.Setup(m => m.GetService(typeof(IEnumerable<IIntegrationEventHandler<SomethingIntegrationEvent>>))).Returns(handlers);

        var consumer = new EventConsumer(logger.Object, serviceProvider.Object);
        var integrationEvent = new SomethingIntegrationEvent();

        // When
        await consumer.ConsumeAsync(integrationEvent, default);

        // Then
        handler.Verify(m => m.HandleAsync(integrationEvent, default), Times.Once());
    }
}
