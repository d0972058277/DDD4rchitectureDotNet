using Architecture.Application.MessageBus;
using Architecture.Domain.MessageBus;

namespace Architecture.Tests.Application.MessageBus;

public class EventPublisherExtensionsTests
{
    [Fact]
    public async Task 擴充方法應該使用IntegrationEvent_Create生成VO後用原本的PublishAsync進行發布()
    {
        // Given
        var somethingIntegrationEvent = new SomethingIntegrationEvent();
        var integrationEvent = IntegrationEvent.Create(somethingIntegrationEvent);
        var eventBus = new Mock<IEventPublisher>();

        // When
        await eventBus.Object.PublishAsync(somethingIntegrationEvent, default);

        // Then
        eventBus.Verify(m => m.PublishAsync(It.Is<IntegrationEvent>(e => e == integrationEvent), default), Times.Once());
    }
}
