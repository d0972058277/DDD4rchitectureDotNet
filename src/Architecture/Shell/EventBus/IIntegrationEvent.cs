namespace Architecture.Shell.EventBus;

public interface IIntegrationEvent
{
    Guid Id { get; init; }
    DateTime CreationTimestamp { get; init; }
}
