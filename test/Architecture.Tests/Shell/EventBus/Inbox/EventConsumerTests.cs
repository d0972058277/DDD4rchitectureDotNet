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
        var type = handler.Object.GetType();
        var types = new Type[] { type, type };

        var serviceProvider = new Mock<IServiceProvider>();
        var serviceScopeFactory = new Mock<IServiceScopeFactory>();
        var serviceScope = new Mock<IServiceScope>();
        var cache = new Mock<IIntegrationEventHandlerTypeCache>();

        serviceScopeFactory.Setup(m => m.CreateScope()).Returns(serviceScope.Object);
        serviceScope.Setup(m => m.ServiceProvider).Returns(serviceProvider.Object);
        serviceProvider.Setup(m => m.GetService(type)).Returns(handler.Object);
        cache.Setup(m => m.GetImpHandlerTypes(typeof(SomethingIntegrationEvent))).Returns(types);

        var consumer = new EventConsumer(cache.Object, serviceScopeFactory.Object, logger.Object);
        var integrationEvent = new SomethingIntegrationEvent();

        // When
        await consumer.ConsumeAsync(integrationEvent, default);

        // Then
        serviceScopeFactory.Verify(m => m.CreateScope(), Times.Exactly(2));
        serviceScope.Verify(m => m.ServiceProvider, Times.Exactly(2));
        serviceProvider.Verify(m => m.GetService(type), Times.Exactly(2));
        handler.Verify(m => m.HandleAsync(integrationEvent, default), Times.Exactly(2));
    }
}
