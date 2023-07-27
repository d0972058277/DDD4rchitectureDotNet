namespace Architecture.Application.MessageBus.Outbox;

public class OutboxProcessor
{
    private readonly OutboxQueue _queue;
    private readonly IIntegrationEventRepository _repository;

    public OutboxProcessor(OutboxQueue queue, IIntegrationEventRepository repository)
    {
        _queue = queue;
        _repository = repository;
    }

    public void Register(Guid transactionId)
    {
        _queue.Enqueue(transactionId);
    }

    public async Task ProcessAsync(CancellationToken cancellationToken = default)
    {
        var transactionIds = _queue.Dequeue();
        var integrationEventEntries = await _repository.FindAsync(transactionIds, cancellationToken);

        foreach (var integrationEventEntry in integrationEventEntries)
            integrationEventEntry.Progress();
        await _repository.SaveAsync(integrationEventEntries, cancellationToken);

        // TODO: Publish integration event

        foreach (var integrationEventEntry in integrationEventEntries)
            integrationEventEntry.Publish();
        await _repository.SaveAsync(integrationEventEntries, cancellationToken);
    }
}
