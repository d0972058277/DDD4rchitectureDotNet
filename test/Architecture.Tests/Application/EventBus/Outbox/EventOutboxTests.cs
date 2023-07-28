using Architecture.Application.EventBus.Outbox;
using Architecture.Application.UnitOfWork;
using Architecture.Domain.EventBus;
using Architecture.Domain.EventBus.Outbox;

namespace Architecture.Tests.Application.EventBus.Outbox
{
    public class EventOutboxTests
    {
        [Fact]
        public async Task 假如OutboxEventPublisher的UnitOfWork沒有活躍的Transaction_應該拋出InvalidOperationException的例外()
        {
            // Given
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(m => m.HasActiveTransaction).Returns(false);

            var repository = new Mock<IIntegrationEventRepository>();

            var eventOutbox = new EventOutbox(unitOfWork.Object, repository.Object);
            var somethingIntegrationEvent = new SomethingIntegrationEvent();

            // When
            var action = () => eventOutbox.SendAsync(somethingIntegrationEvent);

            // Then
            await action.Should().ThrowAsync<InvalidOperationException>();
        }

        [Fact]
        public async Task 假如OutboxEventPublisher的UnitOfWork有活躍的Transaction_應該順利執行Repository的AddAsync行為()
        {
            // Given
            var transactionId = Guid.NewGuid();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(m => m.TransactionId).Returns(transactionId);
            unitOfWork.Setup(m => m.HasActiveTransaction).Returns(true);

            var repository = new Mock<IIntegrationEventRepository>();

            var eventOutbox = new EventOutbox(unitOfWork.Object, repository.Object);
            var somethingIntegrationEvent = new SomethingIntegrationEvent();
            var payload = Payload.Serialize(somethingIntegrationEvent);

            // When
            await eventOutbox.SendAsync(somethingIntegrationEvent);

            // Then
            repository.Verify(m => m.AddAsync(It.Is<IntegrationEventEntry>(e => e.GetPayload() == payload && e.TransactionId == transactionId), default), Times.Once());
        }
    }
}