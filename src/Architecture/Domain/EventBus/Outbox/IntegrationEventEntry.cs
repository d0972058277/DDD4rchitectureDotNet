namespace Architecture.Domain.EventBus.Outbox;

public class IntegrationEventEntry : AggregateRoot<Guid>
{
    private IntegrationEventEntry(Guid id, DateTime creationTimestamp, string typeName, string content, State state, Guid transactionId) : base(id)
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

    public static IntegrationEventEntry Raise(Payload payload, Guid transactionId)
    {
        var state = State.Raised;

        return new IntegrationEventEntry
        (
            payload.Id,
            payload.CreationTimestamp,
            payload.TypeName,
            payload.Content,
            state,
            transactionId
        );
    }

    public void Progress()
    {
        State = State.InProgress;
    }

    public void Publish()
    {
        State = State.Published;
    }
}
