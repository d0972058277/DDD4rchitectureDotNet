using Architecture.Application.MessageBus.Inbox;
using Architecture.Domain.MessageBus;
using Architecture.Domain.MessageBus.Inbox;

namespace Architecture.Tests.Application.MessageBus.Inbox;

public class InboxEventSubscriberTests
{
    [Fact]
    public async Task 應該順利執行Repository的AddAsync行為()
    {
        // Given
        var repository = new Mock<IIntegrationEventRepository>();

        var inboxEventSubscriber = new InboxEventSubscriber(repository.Object);
        var somethingIntegrationEvent = new SomethingIntegrationEvent();
        var integrationEvent = IntegrationEvent.Create(somethingIntegrationEvent);

        // When
        await inboxEventSubscriber.SubscribeAsync(integrationEvent);

        // Then
        repository.Verify(m => m.AddAsync(It.Is<IntegrationEventEntry>(e => e.Id == integrationEvent.Id), default), Times.Once());
    }
}
