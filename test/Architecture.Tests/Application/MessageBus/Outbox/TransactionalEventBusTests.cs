using Architecture.Application.MessageBus.Outbox;
using Architecture.Application.UnitOfWork;
using Architecture.Domain.MessageBus;
using Architecture.Domain.MessageBus.Outbox;

namespace Architecture.Tests.Application.MessageBus.Outbox
{
    public class TransactionalEventBusTests
    {
        [Fact]
        public async Task 假如ITransactionalOutbox的UnitOfWork沒有活躍的Transaction_應該拋出InvalidOperationException的例外()
        {
            // Given
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(m => m.HasActiveTransaction).Returns(false);

            var repository = new Mock<IIntegrationEventRepository>();

            var transactionalEventBus = new TransactionalEventBus(unitOfWork.Object, repository.Object);
            var somethingIntegrationEvent = new SomethingIntegrationEvent();
            var integrationEvent = IntegrationEvent.Create(somethingIntegrationEvent);

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
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(m => m.TransactionId).Returns(transactionId);
            unitOfWork.Setup(m => m.HasActiveTransaction).Returns(true);

            var repository = new Mock<IIntegrationEventRepository>();

            var transactionalEventBus = new TransactionalEventBus(unitOfWork.Object, repository.Object);
            var somethingIntegrationEvent = new SomethingIntegrationEvent();
            var integrationEvent = IntegrationEvent.Create(somethingIntegrationEvent);

            // When
            await transactionalEventBus.PublishAsync(integrationEvent);

            // Then
            repository.Verify(m => m.AddAsync(It.Is<IntegrationEventEntry>(e => e.Id == integrationEvent.Id && e.TransactionId == transactionId), default), Times.Once());
        }
    }
}