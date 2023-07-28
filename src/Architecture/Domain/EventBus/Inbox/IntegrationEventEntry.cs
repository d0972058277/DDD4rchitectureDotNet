namespace Architecture.Domain.EventBus.Inbox;

public class IntegrationEventEntry : AggregateRoot<Guid>
{
    private IntegrationEventEntry(Guid id, DateTime creationTimestamp, string typeName, string message, State state) : base(id)
    {
        CreationTimestamp = creationTimestamp;
        TypeName = typeName;
        Message = message;
        State = state;
    }

    public DateTime CreationTimestamp { get; private set; }

    public string TypeName { get; private set; } = string.Empty;

    public string Message { get; private set; } = string.Empty;

    public State State { get; private set; }

    public IntegrationEvent GetIntegrationEvent()
    {
        return IntegrationEvent.Create(Id, CreationTimestamp, TypeName, Message);
    }

    public static IntegrationEventEntry Receive(IntegrationEvent integrationEvent)
    {
        var state = State.Received;

        return new IntegrationEventEntry
        (
            integrationEvent.Id,
            integrationEvent.CreationTimestamp,
            integrationEvent.TypeName,
            integrationEvent.Message, state
        );
    }

    public void Progress()
    {
        State = State.InProgress;
    }

    public void Consume()
    {
        State = State.Consumed;
    }
}
