namespace Architecture.Shell.EventBus;

public interface IIntegrationEvent
{
    Guid Id { get; }
    DateTime CreationTimestamp { get; }
}
