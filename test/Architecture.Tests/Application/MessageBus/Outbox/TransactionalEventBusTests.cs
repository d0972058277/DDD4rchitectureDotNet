using Architecture.Application.MessageBus.Outbox;
using Architecture.Application.UnitOfWork;

namespace Architecture.Tests.Application.MessageBus.Outbox
{
    public partial class TransactionalEventBusTests
    {
        [Fact]
        public async Task 假如ITransactionalOutbox的UnitOfWork沒有活躍的Transaction_應該拋出InvalidOperationException的例外()
        {
            // Given
            var unitOfWork = new Mock<IReadonlyUnitOfWork>();
            unitOfWork.Setup(m => m.HasActiveTransaction).Returns(false);

            var outboxTransactor = new Mock<IOutboxTransactor>();
            outboxTransactor.Setup(m => m.UnitOfWork).Returns(unitOfWork.Object);

            var transactionalEventBus = new TransactionalEventBus(outboxTransactor.Object);
            var integrationEvent = new SomethingIntegrationEvent();

            // When
            var action = () => transactionalEventBus.PublishAsync(integrationEvent);

            // Then
            await action.Should().ThrowAsync<InvalidOperationException>();
        }

        [Fact]
        public async Task 假如ITransactionalOutbox的UnitOfWork有活躍的Transaction_應該順利執行ITransactionalOutbox的SaveAsync行為()
        {
            // Given
            var transactionId = Guid.NewGuid();
            var readonlyUnitOfWork = new Mock<IReadonlyUnitOfWork>();
            readonlyUnitOfWork.Setup(m => m.TransactionId).Returns(transactionId);
            readonlyUnitOfWork.Setup(m => m.HasActiveTransaction).Returns(true);

            var outboxTransactor = new Mock<IOutboxTransactor>();
            outboxTransactor.Setup(m => m.UnitOfWork).Returns(readonlyUnitOfWork.Object);

            var transactionalEventBus = new TransactionalEventBus(outboxTransactor.Object);
            var integrationEvent = new SomethingIntegrationEvent();

            // When
            await transactionalEventBus.PublishAsync(integrationEvent);

            // Then
            outboxTransactor.Verify(m => m.SaveAsync(transactionId, integrationEvent, default), Times.Once());
        }
    }
}