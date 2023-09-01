using Architecture.Shell.EventBus;
using Architecture.Shell.EventBus.Inbox;

namespace Architecture.Tests.Shell.EventBus.Inbox;

public class IntegrationEventEntityTests
{
    [Fact]
    public void 應該成功收取IntegrationEventEntity()
    {
        // Given
        var payload = GetPayload();

        // When
        var entry = IntegrationEventEntity.Receive(payload);

        // Then
        entry.Id.Should().Be(payload.Id);
        entry.CreationTimestamp.Should().Be(payload.CreationTimestamp);
        entry.TypeName.Should().Be(payload.TypeName);
        entry.Content.Should().Be(payload.Content);
        entry.State.Should().Be(State.Received);
    }

    [Fact]
    public void 處理後_狀態應該要是Handled()
    {
        // Given
        var entry = GetIntegrationEventEntity();

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

    private static IntegrationEventEntity GetIntegrationEventEntity()
    {
        var payload = GetPayload();
        var entry = IntegrationEventEntity.Receive(payload);
        return entry;
    }
}
