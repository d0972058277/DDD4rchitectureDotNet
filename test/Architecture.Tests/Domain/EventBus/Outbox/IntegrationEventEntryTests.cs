using Architecture.Domain.EventBus;
using Architecture.Domain.EventBus.Outbox;

namespace Architecture.Tests.Domain.EventBus.Outbox;

public class IntegrationEventEntryTests
{
    [Fact]
    public void 應該成功發起IntegrationEventEntry()
    {
        // Given
        var payload = GetPayload();
        var transactionId = Guid.NewGuid();

        // When
        var entry = IntegrationEventEntry.Raise(payload, transactionId);

        // Then
        entry.Id.Should().Be(payload.Id);
        entry.CreationTimestamp.Should().Be(payload.CreationTimestamp);
        entry.TypeName.Should().Be(payload.TypeName);
        entry.Content.Should().Be(payload.Content);
        entry.State.Should().Be(State.Raised);
        entry.TransactionId.Should().Be(transactionId);
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
    public void 發佈後_狀態應該要是Published()
    {
        // Given
        var entry = GetIntegrationEventEntry();
        entry.Progress();

        // When
        entry.Publish();

        // Then
        entry.State.Should().Be(State.Published);
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
        var transactionId = Guid.NewGuid();
        var entry = IntegrationEventEntry.Raise(payload, transactionId);
        return entry;
    }
}
