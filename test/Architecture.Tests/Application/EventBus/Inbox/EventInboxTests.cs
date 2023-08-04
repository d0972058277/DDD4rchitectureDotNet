using Architecture.Application.EventBus.Inbox;
using Architecture.Domain.EventBus;
using Architecture.Domain.EventBus.Inbox;

namespace Architecture.Tests.Application.EventBus.Inbox;

public class EventInboxTests
{
    [Fact]
    public async Task 應該順利執行Repository的AddAsync行為()
    {
        // Given
        var repository = new Mock<IIntegrationEventRepository>();

        var inboxEventConsumer = new EventInbox(repository.Object);
        var somethingIntegrationEvent = new SomethingIntegrationEvent();
        var payload = Payload.Serialize(somethingIntegrationEvent);

        // When
        await inboxEventConsumer.ConsumeAsync(somethingIntegrationEvent);

        // Then
        repository.Verify(m => m.AddAsync(It.Is<IntegrationEventEntry>(e => e.GetPayload() == payload), default), Times.Once());
    }
}
