using Architecture.Application.MessageBus.Outbox;

namespace Architecture.Tests.Application.MessageBus.Outbox
{
    [Collection("Sequential")]
    public class OutboxQueueTests
    {
        [Fact]
        public async Task 高併發進行Enqueue的狀況下_應該有線程安全()
        {
            // Given
            var transactionIds = Enumerable.Range(1, 1000).Select(i => Guid.NewGuid()).ToList();
            var outboxQueue = new OutboxQueue(30);
            outboxQueue.Clear();

            // When
            var tasks = transactionIds.Select(id => Task.Run(() => outboxQueue.Enqueue(id))).ToArray();
            await Task.WhenAll(tasks);

            // Then
            outboxQueue.TransactionIds.Should().BeEquivalentTo(transactionIds);
        }

        [Fact]
        public async Task 高併發進行Dequeue的狀況下_應該有線程安全()
        {
            // Given
            var transactionIds = Enumerable.Range(1, 1000).Select(i => Guid.NewGuid()).ToList();
            var outboxQueue = new OutboxQueue(30);
            outboxQueue.Clear();
            transactionIds.ForEach(id => outboxQueue.Enqueue(id));

            // When
            var tasks = transactionIds.Select(id => Task.Run(() => outboxQueue.Dequeue())).ToArray();
            await Task.WhenAll(tasks);
            var dequeue = tasks.SelectMany(t => t.Result).ToList();

            // Then
            dequeue.Should().BeEquivalentTo(transactionIds);
        }

        [Fact]
        public async Task 高併發同時進行Enqueue與Dequeue的狀況下_應該有線程安全()
        {
            // Given
            var transactionIds = Enumerable.Range(1, 1000).Select(i => Guid.NewGuid()).ToList();
            var outboxQueue = new OutboxQueue(30);
            outboxQueue.Clear();

            // When
            var tasks = transactionIds.Select(id => Task.Run(() =>
            {
                outboxQueue.Enqueue(id);
                return outboxQueue.Dequeue();
            })).ToArray();
            await Task.WhenAll(tasks);
            var dequeue = tasks.SelectMany(t => t.Result).ToList();

            // Then
            dequeue.Should().BeEquivalentTo(transactionIds);
        }
    }
}