namespace Architecture.Application.MessageBus.Outbox;

public class OutboxProcessor
{
    private readonly TransactionalQueue _queue;
    private readonly IIntegrationEventRepository _repository;
    private readonly IEventBroker _messageBroker;

    public OutboxProcessor(TransactionalQueue queue, IIntegrationEventRepository repository, IEventBroker messageBroker)
    {
        _queue = queue;
        _repository = repository;
        _messageBroker = messageBroker;
    }

    public void Enqueue(Guid transactionId)
    {
        _queue.Enqueue(transactionId);
    }

    public async Task PublishAsync(CancellationToken cancellationToken = default)
    {
        var transactionIds = _queue.Dequeue();
        var integrationEventEntries = await _repository.FindAsync(transactionIds, cancellationToken);

        foreach (var integrationEventEntry in integrationEventEntries)
            integrationEventEntry.Progress();
        await _repository.SaveAsync(integrationEventEntries, cancellationToken);

        var integrationEvents = integrationEventEntries.Select(e => e.GetIntegrationEvent()).ToList();
        await _messageBroker.PublishsAsync(integrationEvents, cancellationToken);

        foreach (var integrationEventEntry in integrationEventEntries)
            integrationEventEntry.Publish();
        await _repository.SaveAsync(integrationEventEntries, cancellationToken);
    }
}
