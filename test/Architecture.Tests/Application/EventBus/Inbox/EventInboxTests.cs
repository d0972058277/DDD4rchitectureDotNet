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

        var eventInbox = new EventInbox(repository.Object);
        var somethingIntegrationEvent = new SomethingIntegrationEvent();
        var integrationEvent = IntegrationEvent.Create(somethingIntegrationEvent);

        // When
        await eventInbox.ReceiveAsync(integrationEvent);

        // Then
        repository.Verify(m => m.AddAsync(It.Is<IntegrationEventEntry>(e => e.Id == integrationEvent.Id), default), Times.Once());
    }
}
