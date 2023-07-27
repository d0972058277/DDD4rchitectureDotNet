using System.Collections.Concurrent;

namespace Architecture.Application.MessageBus.Inbox;

public sealed class InboxQueue
{
    private readonly ConcurrentQueue<Guid> _integrationEventIdQueue = new();

    public InboxQueue(int slidingWindowSize)
    {
        SlidingWindowSize = slidingWindowSize;
    }

    public int SlidingWindowSize { get; }

    public IReadOnlyCollection<Guid> IntegrationEventIds => _integrationEventIdQueue;

    public void Enqueue(Guid transactionId)
    {
        _integrationEventIdQueue.Enqueue(transactionId);
    }

    public IReadOnlyList<Guid> Dequeue()
    {
        var integrationEventIds = new List<Guid>();

        for (int i = 0; i < SlidingWindowSize; i++)
        {
            if (_integrationEventIdQueue.TryDequeue(out var integrationEventId))
            {
                integrationEventIds.Add(integrationEventId);
            }
            else
            {
                break;
            }
        }

        return integrationEventIds;
    }

    public void Clear()
    {
        _integrationEventIdQueue.Clear();
    }
}
