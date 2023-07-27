namespace Architecture.Application.MessageBus.Inbox;

public class InboxProcessor
{
    private readonly InboxQueue _queue;
    private readonly IIntegrationEventRepository _repository;

    public InboxProcessor(InboxQueue queue, IIntegrationEventRepository repository)
    {
        _queue = queue;
        _repository = repository;
    }

    public void Register(Guid integrationEventId)
    {
        _queue.Enqueue(integrationEventId);
    }

    public async Task ProcessAsync(CancellationToken cancellationToken = default)
    {
        var integrationEventIds = _queue.Dequeue();
        var integrationEventEntries = await _repository.FindAsync(integrationEventIds, cancellationToken);

        foreach (var integrationEventEntry in integrationEventEntries)
            integrationEventEntry.Progress();
        await _repository.SaveAsync(integrationEventEntries, cancellationToken);

        // TODO: Consume integration event

        foreach (var integrationEventEntry in integrationEventEntries)
            integrationEventEntry.Consume();
        await _repository.SaveAsync(integrationEventEntries, cancellationToken);
    }
}
