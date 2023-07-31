using Architecture.Domain.EventBus;
using Architecture.Domain.EventBus.Inbox;

namespace Architecture.Application.EventBus.Inbox;

public class InboxEventConsumer : IEventConsumer
{
    private readonly IIntegrationEventRepository _repository;

    public InboxEventConsumer(IIntegrationEventRepository repository)
    {
        _repository = repository;
    }

    public Task ConsumeAsync<T>(T integrationEvent, CancellationToken cancellationToken = default) where T : IIntegrationEvent
    {
        var payload = Payload.Serialize(integrationEvent);
        var entry = IntegrationEventEntry.Receive(payload);
        return _repository.AddAsync(entry, cancellationToken);
    }
}
