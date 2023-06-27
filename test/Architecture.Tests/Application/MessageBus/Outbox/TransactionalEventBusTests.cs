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
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            mockUnitOfWork.Setup(m => m.HasActiveTransaction).Returns(false);

            var mockTransactionalOutbox = new Mock<ITransactionalOutbox>();
            mockTransactionalOutbox.Setup(m => m.UnitOfWork).Returns(mockUnitOfWork.Object);

            var transactionalEventBus = new TransactionalEventBus(mockTransactionalOutbox.Object);
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
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            mockUnitOfWork.Setup(m => m.HasActiveTransaction).Returns(true);

            var mockTransactionalOutbox = new Mock<ITransactionalOutbox>();
            mockTransactionalOutbox.Setup(m => m.UnitOfWork).Returns(mockUnitOfWork.Object);

            var transactionalEventBus = new TransactionalEventBus(mockTransactionalOutbox.Object);
            var integrationEvent = new SomethingIntegrationEvent();

            // When
            await transactionalEventBus.PublishAsync(integrationEvent);

            // Then
            mockTransactionalOutbox.Verify(m => m.SaveAsync(integrationEvent, default), Times.Once());
        }
    }
}