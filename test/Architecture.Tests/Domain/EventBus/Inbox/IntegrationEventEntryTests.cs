using Architecture.Domain.EventBus;
using Architecture.Domain.EventBus.Inbox;

namespace Architecture.Tests.Domain.EventBus.Inbox;

public class IntegrationEventEntryTests
{
    [Fact]
    public void 應該成功收取IntegrationEventEntry()
    {
        // Given
        var integrationEvent = GetIntegrationEvent();

        // When
        var integrationEventEntry = IntegrationEventEntry.Receive(integrationEvent);

        // Then
        integrationEventEntry.Id.Should().Be(integrationEvent.Id);
        integrationEventEntry.CreationTimestamp.Should().Be(integrationEvent.CreationTimestamp);
        integrationEventEntry.TypeName.Should().Be(integrationEvent.TypeName);
        integrationEventEntry.Message.Should().Be(integrationEvent.Message);
        integrationEventEntry.State.Should().Be(State.Received);
    }

    [Fact]
    public void 安排動作後_狀態應該是InProgress()
    {
        // Given
        var integrationEvent = GetIntegrationEventEntry();

        // When
        integrationEvent.Progress();

        // Then
        integrationEvent.State.Should().Be(State.InProgress);
    }

    [Fact]
    public void 處理後_狀態應該要是Consumed()
    {
        // Given
        var integrationEvent = GetIntegrationEventEntry();
        integrationEvent.Progress();

        // When
        integrationEvent.Consume();

        // Then
        integrationEvent.State.Should().Be(State.Consumed);
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
        var integrationEventEntry = IntegrationEventEntry.Receive(integrationEvent);
        return integrationEventEntry;
    }
}
