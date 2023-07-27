namespace Architecture.Domain.MessageBus.Outbox;

public class IntegrationEventEntry : AggregateRoot<Guid>, IIntegrationEvent
{
    private IntegrationEventEntry(Guid id, DateTime creationTimestamp, string typeName, string message, State state, int timesSent, Guid transactionId) : base(id)
    {
        CreationTimestamp = creationTimestamp;
        TypeName = typeName;
        Message = message;
        State = state;
        TimesSent = timesSent;
        TransactionId = transactionId;
    }

    public DateTime CreationTimestamp { get; private set; }

    public string TypeName { get; private set; } = string.Empty;

    public string Message { get; private set; } = string.Empty;

    public State State { get; private set; }

    public int TimesSent { get; private set; }

    public Guid TransactionId { get; private set; }

    public IntegrationEvent GetIntegrationEvent()
    {
        return IntegrationEvent.Create(Id, CreationTimestamp, TypeName, Message);
    }

    public static IntegrationEventEntry Raise(IntegrationEvent integrationEvent, Guid transactionId)
    {
        var state = State.Raised;
        var timesSent = 0;

        return new IntegrationEventEntry
        (
            integrationEvent.Id,
            integrationEvent.CreationTimestamp,
            integrationEvent.TypeName,
            integrationEvent.Message,
            state,
            timesSent,
            transactionId
        );
    }

    public void Progress()
    {
        State = State.InProgress;
        TimesSent++;
    }

    public void Publish()
    {
        State = State.Published;
    }
}
