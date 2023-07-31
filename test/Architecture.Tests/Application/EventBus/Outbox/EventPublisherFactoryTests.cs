using Architecture.Application.EventBus;
using Architecture.Application.EventBus.Outbox;
using Architecture.Application.UnitOfWork;

namespace Architecture.Tests.Application.EventBus.Outbox;

public class EventPublisherFactoryTests
{
    [Fact]
    public void 應該取得最後一筆實作IEventPublisher且非EventPublisher的註冊物件()
    {
        // Given
        var unitOfWork = new Mock<IUnitOfWork>();
        var repository = new Mock<IIntegrationEventRepository>();
        var eventOutBox = new OutboxEventPublisher(unitOfWork.Object, repository.Object);
        var secondEventPublisher = new Mock<IEventPublisher>();
        var lastEventPublisher = new Mock<IEventPublisher>();
        var eventPublisheres = new List<IEventPublisher> { eventOutBox, secondEventPublisher.Object, lastEventPublisher.Object };
        var eventPublisherFactory = new EventPublisherFactory(eventPublisheres);

        // When
        var realityEventPublisher = eventPublisherFactory.GetRealityEventPublisher();

        // Then
        realityEventPublisher.Should().Be(lastEventPublisher.Object);
    }

    [Fact]
    public void 如果找不到最後一筆實作IEventPublisher且非EventPublisher的註冊物件_應該拋出InvalidOperationException()
    {
        // Given
        var unitOfWork = new Mock<IUnitOfWork>();
        var repository = new Mock<IIntegrationEventRepository>();
        var eventOutBox = new OutboxEventPublisher(unitOfWork.Object, repository.Object);
        var eventPublisheres = new List<IEventPublisher> { eventOutBox };
        var eventPublisherFactory = new EventPublisherFactory(eventPublisheres);

        // When
        var func = () => eventPublisherFactory.GetRealityEventPublisher();

        // Then
        func.Should().Throw<InvalidOperationException>();
    }
}
