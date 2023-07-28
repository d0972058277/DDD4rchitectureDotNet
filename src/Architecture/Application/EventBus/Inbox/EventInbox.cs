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

    public Task ReceiveAsync(IntegrationEvent integrationEvent, CancellationToken cancellationToken = default)
    {
        var integrationEventEntry = IntegrationEventEntry.Receive(integrationEvent);
        return _repository.AddAsync(integrationEventEntry, cancellationToken);
    }
}
