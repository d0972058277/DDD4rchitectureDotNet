using Architecture.Domain.MessageBus;
using Architecture.Domain.MessageBus.Inbox;

namespace Architecture.Application.MessageBus.Inbox;

public class InboxEventSubscriber : IEventSubscriber
{
    private readonly IIntegrationEventRepository _repository;

    public InboxEventSubscriber(IIntegrationEventRepository repository)
    {
        _repository = repository;
    }

    public Task SubscribeAsync(IntegrationEvent integrationEvent, CancellationToken cancellationToken = default)
    {
        var integrationEventEntry = IntegrationEventEntry.Subscribe(integrationEvent);
        return _repository.AddAsync(integrationEventEntry, cancellationToken);
    }
}
