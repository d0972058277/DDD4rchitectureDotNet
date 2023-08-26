using Architecture.Shell;
using Architecture.Shell.EventBus;
using Architecture.Shell.EventBus.Outbox;

namespace Architecture.Tests.Shell.EventBus.Outbox
{
    public class OutboxTests
    {
        [Fact]
        public async Task 假如Outbox的UnitOfWork沒有活躍的Transaction_應該拋出InvalidOperationException的例外()
        {
            // Given
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(m => m.HasActiveTransaction).Returns(false);

            var repository = new Mock<IIntegrationEventRepository>();

            var outbox = new Architecture.Shell.EventBus.Outbox.Outbox(unitOfWork.Object, repository.Object);
            var somethingIntegrationEvent = new SomethingIntegrationEvent();

            // When
            var action = () => outbox.PublishAsync(somethingIntegrationEvent);

            // Then
            await action.Should().ThrowAsync<InvalidOperationException>();
        }

        [Fact]
        public async Task 假如Outbox的UnitOfWork有活躍的Transaction_應該順利執行Repository的AddAsync行為()
        {
            // Given
            var transactionId = Guid.NewGuid();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(m => m.TransactionId).Returns(transactionId);
            unitOfWork.Setup(m => m.HasActiveTransaction).Returns(true);

            var repository = new Mock<IIntegrationEventRepository>();

            var outbox = new Architecture.Shell.EventBus.Outbox.Outbox(unitOfWork.Object, repository.Object);
            var somethingIntegrationEvent = new SomethingIntegrationEvent();
            var payload = Payload.Serialize(somethingIntegrationEvent);

            // When
            await outbox.PublishAsync(somethingIntegrationEvent);

            // Then
            repository.Verify(m => m.AddAsync(It.Is<IntegrationEventEntity>(e => e.GetPayload() == payload && e.TransactionId == transactionId), default), Times.Once());
        }
    }
}