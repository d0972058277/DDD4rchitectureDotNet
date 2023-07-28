using Architecture.Domain.EventBus;
using Architecture.Domain.EventBus.Inbox;

namespace Architecture.Tests.Domain.EventBus.Inbox;

public class IntegrationEventEntryTests
{
    [Fact]
    public void 應該成功收取IntegrationEventEntry()
    {
        // Given
        var payload = GetPayload();

        // When
        var entry = IntegrationEventEntry.Receive(payload);

        // Then
        entry.Id.Should().Be(payload.Id);
        entry.CreationTimestamp.Should().Be(payload.CreationTimestamp);
        entry.TypeName.Should().Be(payload.TypeName);
        entry.Content.Should().Be(payload.Content);
        entry.State.Should().Be(State.Received);
    }

    [Fact]
    public void 安排動作後_狀態應該是InProgress()
    {
        // Given
        var entry = GetIntegrationEventEntry();

        // When
        entry.Progress();

        // Then
        entry.State.Should().Be(State.InProgress);
    }

    [Fact]
    public void 處理後_狀態應該要是Handled()
    {
        // Given
        var entry = GetIntegrationEventEntry();
        entry.Progress();

        // When
        entry.Handle();

        // Then
        entry.State.Should().Be(State.Handled);
    }

    private static Payload GetPayload()
    {
        var somethingIntegrationEvent = new SomethingIntegrationEvent();
        var payload = Payload.Serialize(somethingIntegrationEvent);
        return payload;
    }

    private static IntegrationEventEntry GetIntegrationEventEntry()
    {
        var payload = GetPayload();
        var entry = IntegrationEventEntry.Receive(payload);
        return entry;
    }
}
