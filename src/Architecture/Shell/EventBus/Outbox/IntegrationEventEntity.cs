using Architecture.Core;

namespace Architecture.Shell.EventBus.Outbox;

public class IntegrationEventEntity : AggregateRoot<Guid>
{
    private IntegrationEventEntity(Guid id, DateTime creationTimestamp, string typeName, string content, State state, Guid transactionId) : base(id)
    {
        CreationTimestamp = creationTimestamp;
        TypeName = typeName;
        Content = content;
        State = state;
        TransactionId = transactionId;
    }

    public DateTime CreationTimestamp { get; private set; }

    public string TypeName { get; private set; } = string.Empty;

    public string Content { get; private set; } = string.Empty;

    public State State { get; private set; }

    public Guid TransactionId { get; private set; }

    public Payload GetPayload()
    {
        return Payload.Create(Id, CreationTimestamp, TypeName, Content);
    }

    public static IntegrationEventEntity Raise(Payload payload, Guid transactionId)
    {
        var state = State.Raised;

        return new IntegrationEventEntity
        (
            payload.Id,
            payload.CreationTimestamp,
            payload.TypeName,
            payload.Content,
            state,
            transactionId
        );
    }

    public void Publish()
    {
        State = State.Published;
    }
}
