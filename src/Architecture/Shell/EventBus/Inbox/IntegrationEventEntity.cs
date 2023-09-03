using Architecture.Core;

namespace Architecture.Shell.EventBus.Inbox;

public class IntegrationEventEntity : AggregateRoot<Guid>
{
    private IntegrationEventEntity(Guid id, DateTime creationTimestamp, string typeName, string content, State state) : base(id)
    {
        CreationTimestamp = creationTimestamp;
        TypeName = typeName;
        Content = content;
        State = state;
    }

    public DateTime CreationTimestamp { get; private set; }

    public string TypeName { get; private set; } = string.Empty;

    public string Content { get; private set; } = string.Empty;

    public State State { get; private set; }

    public Payload GetPayload()
    {
        return Payload.Create(Id, CreationTimestamp, TypeName, Content);
    }

    public static IntegrationEventEntity Receive(Payload payload)
    {
        var state = State.Received;

        return new IntegrationEventEntity
        (
            payload.Id,
            payload.CreationTimestamp,
            payload.TypeName,
            payload.Content, state
        );
    }

    public void Handle()
    {
        State = State.Handled;
    }
}
