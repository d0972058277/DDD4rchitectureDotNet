using Architecture.Application.EventBus;
using Architecture.Application.EventBus.Outbox;
using Architecture.Application.UnitOfWork;
using Microsoft.Extensions.Logging;

namespace Architecture.Tests.Application.EventBus.Outbox;

public class UnitOfWorkOutboxDecoratorTests
{
    [Fact]
    public async Task 執行CommitAsync後_應該執行OutboxProcess來執行整合事件的發佈進程()
    {
        // Given
        var transactionId = Guid.NewGuid();
        var unitOfWork = new Mock<IUnitOfWork>();
        unitOfWork.Setup(m => m.BeginTransactionAsync(It.IsAny<CancellationToken>())).Callback(() =>
        {
            unitOfWork.Setup(m => m.TransactionId).Returns(transactionId);
            unitOfWork.Setup(m => m.HasActiveTransaction).Returns(true);
        });

        var outboxProcessor = new Mock<IOutboxProcessor>();
        var logger = new Mock<ILogger<UnitOfWorkOutboxDecorator>>();

        var decorator = new UnitOfWorkOutboxDecorator(unitOfWork.Object, outboxProcessor.Object, logger.Object);
        await decorator.BeginTransactionAsync(default);

        // When
        await decorator.CommitAsync(default);

        // Then
        outboxProcessor.Verify(m => m.ProcessAsync(transactionId, default), Times.Once());
    }

    [Fact]
    public async Task 沒有活躍的_Transaction_應該拋出InvalidOperationException()
    {
        // Given
        var transactionId = Guid.NewGuid();
        var unitOfWork = new Mock<IUnitOfWork>();
        unitOfWork.Setup(m => m.BeginTransactionAsync(It.IsAny<CancellationToken>())).Callback(() =>
        {
            unitOfWork.Setup(m => m.TransactionId).Returns(transactionId);
            unitOfWork.Setup(m => m.HasActiveTransaction).Returns(true);
        });

        var outboxProcessor = new Mock<IOutboxProcessor>();
        var logger = new Mock<ILogger<UnitOfWorkOutboxDecorator>>();

        var decorator = new UnitOfWorkOutboxDecorator(unitOfWork.Object, outboxProcessor.Object, logger.Object);

        // When
        var func = () => decorator.CommitAsync(default);

        // Then
        await func.Should().ThrowAsync<InvalidOperationException>();
    }
}
