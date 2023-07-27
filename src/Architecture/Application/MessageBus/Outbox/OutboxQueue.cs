using System.Collections.Concurrent;

namespace Architecture.Application.MessageBus.Outbox
{
    public sealed class OutboxQueue
    {
        private readonly ConcurrentQueue<Guid> _transactionIdQueue = new();

        public OutboxQueue(int slidingWindowSize)
        {
            SlidingWindowSize = slidingWindowSize;
        }

        public int SlidingWindowSize { get; }

        public IReadOnlyCollection<Guid> TransactionIds => _transactionIdQueue;

        public void Enqueue(Guid transactionId)
        {
            _transactionIdQueue.Enqueue(transactionId);
        }

        public IReadOnlyList<Guid> Dequeue()
        {
            var transactionIds = new List<Guid>();

            for (int i = 0; i < SlidingWindowSize; i++)
            {
                if (_transactionIdQueue.TryDequeue(out var transactionId))
                {
                    transactionIds.Add(transactionId);
                }
                else
                {
                    break;
                }
            }

            return transactionIds;
        }

        public void Clear()
        {
            _transactionIdQueue.Clear();
        }
    }
}