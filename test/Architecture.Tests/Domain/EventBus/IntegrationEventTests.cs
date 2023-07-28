using Architecture.Domain.EventBus;

namespace Architecture.Tests.Domain.EventBus;

public class IntegrationEventTests
{
    [Fact]
    public void 應該成功建立IntegrationEvent()
    {
        // Given
        var somethingIntegrationEvent = new SomethingIntegrationEvent
        {
            Id = Guid.Empty,
            CreationTimestamp = new DateTime(1, 1, 1)
        };

        // When
        var integrationEvent = IntegrationEvent.Create(somethingIntegrationEvent);

        // Then
        integrationEvent.Id.Should().Be(somethingIntegrationEvent.Id);
        integrationEvent.CreationTimestamp.Should().Be(somethingIntegrationEvent.CreationTimestamp);
        integrationEvent.TypeName.Should().Be("Architecture.Tests.SomethingIntegrationEvent");
        integrationEvent.Message.Should().Be("{\"Id\":\"00000000-0000-0000-0000-000000000000\",\"CreationTimestamp\":\"0001-01-01T00:00:00\"}");
    }
}
