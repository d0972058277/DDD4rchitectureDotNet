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

    public Task ReceiveAsync<T>(T integrationEvent, CancellationToken cancellationToken = default) where T : IIntegrationEvent
    {
        var payload = Payload.Serialize(integrationEvent);
        var entry = IntegrationEventEntry.Receive(payload);
        return _repository.AddAsync(entry, cancellationToken);
    }
}
