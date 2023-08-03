using Architecture.Domain.EventBus;
using Architecture.Domain.EventBus.Inbox;

namespace Architecture.Application.EventBus.Inbox;

public class EventInbox : IEventInbox
{
    private readonly IIntegrationEventRepository _repository;

    public EventInbox(IIntegrationEventRepository repository)
    {
        _repository = repository;
    }

    public Task ConsumeAsync<TIntegrationEvent>(TIntegrationEvent integrationEvent, CancellationToken cancellationToken = default) where TIntegrationEvent : IIntegrationEvent
    {
        var payload = Payload.Serialize(integrationEvent);
        var entry = IntegrationEventEntry.Receive(payload);
        return _repository.AddAsync(entry, cancellationToken);
    }
}
