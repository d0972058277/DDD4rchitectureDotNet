namespace Architecture.Domain.EventBus;

public interface IIntegrationEvent
{
    Guid Id { get; }
    DateTime CreationTimestamp { get; }
}
