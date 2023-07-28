using Architecture.Application.EventBus;
using Architecture.Domain.EventBus;

namespace Architecture.Tests.Application.EventBus;

public class EventPublisherExtensionsTests
{
    [Fact]
    public async Task 擴充方法應該使用IntegrationEvent_Create生成VO後用原本的PublishAsync進行發布()
    {
        // Given
        var somethingIntegrationEvent = new SomethingIntegrationEvent();
        var integrationEvent = IntegrationEvent.Create(somethingIntegrationEvent);
        var outbox = new Mock<IEventOutbox>();

        // When
        await outbox.Object.PublishAsync(somethingIntegrationEvent, default);

        // Then
        outbox.Verify(m => m.SendAsync(It.Is<IntegrationEvent>(e => e == integrationEvent), default), Times.Once());
    }
}
