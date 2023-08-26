namespace Architecture.Shell.EventBus.Inbox;

public class Inbox : IInbox
{
    private readonly IIntegrationEventRepository _repository;

    public Inbox(IIntegrationEventRepository repository)
    {
        _repository = repository;
    }

    public Task ConsumeAsync<TIntegrationEvent>(TIntegrationEvent integrationEvent, CancellationToken cancellationToken = default) where TIntegrationEvent : IIntegrationEvent
    {
        var payload = Payload.Serialize(integrationEvent);
        var entry = IntegrationEventEntity.Receive(payload);
        return _repository.AddAsync(entry, cancellationToken);
    }
}
