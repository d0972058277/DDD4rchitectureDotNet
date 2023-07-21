using Architecture.Application.MessageBus.Outbox;

namespace Architecture.Tests.Application.MessageBus.Outbox
{
    [Collection("Sequential")]
    public class TransactionalQueueTests
    {
        [Fact]
        public async Task 高併發進行Enqueue的狀況下_應該有線程安全()
        {
            // Given
            var transactionIds = Enumerable.Range(1, 1000).Select(i => Guid.NewGuid()).ToList();
            var transactionalQueue = TransactionalQueue.Instance;
            transactionalQueue.Clear();

            // When
            var tasks = transactionIds.Select(id => Task.Run(() => transactionalQueue.Enqueue(id))).ToArray();
            await Task.WhenAll(tasks);

            // Then
            transactionalQueue.TransactionIds.Should().BeEquivalentTo(transactionIds);
        }

        [Fact]
        public async Task 高併發進行Dequeue的狀況下_應該有線程安全()
        {
            // Given
            var transactionIds = Enumerable.Range(1, 1000).Select(i => Guid.NewGuid()).ToList();
            var transactionalQueue = TransactionalQueue.Instance;
            transactionalQueue.Clear();
            transactionIds.ForEach(id => transactionalQueue.Enqueue(id));

            // When
            var tasks = transactionIds.Select(id => Task.Run(() => transactionalQueue.Dequeue())).ToArray();
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
            var transactionalQueue = TransactionalQueue.Instance;
            transactionalQueue.Clear();

            // When
            var tasks = transactionIds.Select(id => Task.Run(() =>
            {
                transactionalQueue.Enqueue(id);
                return transactionalQueue.Dequeue();
            })).ToArray();
            await Task.WhenAll(tasks);
            var dequeue = tasks.SelectMany(t => t.Result).ToList();

            // Then
            dequeue.Should().BeEquivalentTo(transactionIds);
        }
    }
}