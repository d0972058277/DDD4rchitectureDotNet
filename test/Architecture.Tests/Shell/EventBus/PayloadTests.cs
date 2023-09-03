using Architecture.Shell.EventBus;

namespace Architecture.Tests.Shell.EventBus;

public class PayloadTests
{
    [Fact]
    public void 應該成功序列化IntegrationEvent()
    {
        // Given
        var somethingIntegrationEvent = new SomethingIntegrationEvent
        {
            Id = Guid.Empty,
            CreationTimestamp = new DateTime(1, 1, 1)
        };

        // When
        var payload = Payload.Serialize(somethingIntegrationEvent);

        // Then
        payload.Id.Should().Be(somethingIntegrationEvent.Id);
        payload.CreationTimestamp.Should().Be(somethingIntegrationEvent.CreationTimestamp);
        payload.TypeName.Should().Be("Architecture.Tests.SomethingIntegrationEvent");
        payload.Content.Should().Be("{\"Id\":\"00000000-0000-0000-0000-000000000000\",\"CreationTimestamp\":\"0001-01-01T00:00:00\"}");
    }

    [Fact]
    public void 應該可以成功反序列化成IIntegrationEvent()
    {
        // Given
        var payload = GetPayload();

        // When
        var realIntegrationEvent = payload.Deserialize();

        // Then
        realIntegrationEvent.As<SomethingIntegrationEvent>().Id.Should().Be(payload.Id);
        realIntegrationEvent.As<SomethingIntegrationEvent>().CreationTimestamp.Should().Be(payload.CreationTimestamp);
    }

    [Fact]
    public void 應該可以成功反序列化成明確的IntegrationEvent()
    {
        // Given
        var payload = GetPayload();

        // When
        var realIntegrationEvent = payload.Deserialize<SomethingIntegrationEvent>();

        // Then
        realIntegrationEvent.Id.Should().Be(payload.Id);
        realIntegrationEvent.CreationTimestamp.Should().Be(payload.CreationTimestamp);
    }

    private static Payload GetPayload()
    {
        var somethingIntegrationEvent = new SomethingIntegrationEvent();
        var payload = Payload.Serialize(somethingIntegrationEvent);
        return payload;
    }
}
