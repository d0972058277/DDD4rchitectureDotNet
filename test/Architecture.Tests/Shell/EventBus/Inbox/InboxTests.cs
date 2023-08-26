using Architecture.Shell.EventBus;
using Architecture.Shell.EventBus.Inbox;

namespace Architecture.Tests.Shell.EventBus.Inbox;

public class InboxTests
{
    [Fact]
    public async Task 應該順利執行Repository的AddAsync行為()
    {
        // Given
        var repository = new Mock<IIntegrationEventRepository>();

        var inboxEventConsumer = new Architecture.Shell.EventBus.Inbox.Inbox(repository.Object);
        var somethingIntegrationEvent = new SomethingIntegrationEvent();
        var payload = Payload.Serialize(somethingIntegrationEvent);

        // When
        await inboxEventConsumer.ConsumeAsync(somethingIntegrationEvent);

        // Then
        repository.Verify(m => m.AddAsync(It.Is<IntegrationEventEntity>(e => e.GetPayload() == payload), default), Times.Once());
    }
}
