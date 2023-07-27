using Architecture.Domain.MessageBus.Outbox;

namespace Architecture.Tests.Domain.MessageBus.Outbox;

public class IntegrationEventEntryTests
{
    [Fact]
    public void 應該成功建立IntegrationEvent()
    {
        // Given
        var integrationEvent = GetIntegrationEvent();
        var transactionId = Guid.NewGuid();

        // When
        var integrationEventEntry = IntegrationEventEntry.Raise(integrationEvent, transactionId);

        // Then
        integrationEventEntry.Id.Should().Be(integrationEvent.Id);
        integrationEventEntry.CreationTimestamp.Should().Be(integrationEvent.CreationTimestamp);
        integrationEventEntry.TypeName.Should().Be(integrationEvent.TypeName);
        integrationEventEntry.Message.Should().Be(integrationEvent.Message);
        integrationEventEntry.State.Should().Be(State.Raised);
        integrationEventEntry.TimesSent.Should().Be(0);
        integrationEventEntry.TransactionId.Should().Be(transactionId);
    }

    [Fact]
    public void 安派動作後_狀態應該是InProgress_且TimesSent要加1()
    {
        // Given
        var integrationEvent = GetIntegrationEventEntry();

        // When
        integrationEvent.Progress();

        // Then
        integrationEvent.State.Should().Be(State.InProgress);
        integrationEvent.TimesSent.Should().Be(1);
    }

    [Fact]
    public void 發佈後_狀態應該要是Published()
    {
        // Given
        var integrationEvent = GetIntegrationEventEntry();
        integrationEvent.Progress();

        // When
        integrationEvent.Publish();

        // Then
        integrationEvent.State.Should().Be(State.Published);
    }

    private static IntegrationEvent GetIntegrationEvent()
    {
        var somethingIntegrationEvent = new SomethingIntegrationEvent();
        var integrationEvent = IntegrationEvent.Create(somethingIntegrationEvent);
        return integrationEvent;
    }

    private static IntegrationEventEntry GetIntegrationEventEntry()
    {
        var integrationEvent = GetIntegrationEvent();
        var transactionId = Guid.NewGuid();
        var integrationEventEntry = IntegrationEventEntry.Raise(integrationEvent, transactionId);
        return integrationEventEntry;
    }
}
