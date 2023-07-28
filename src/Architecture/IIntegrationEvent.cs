namespace Architecture;

public interface IIntegrationEvent
{
    Guid Id { get; }
    DateTime CreationTimestamp { get; }
}
