using Architecture.Application.EventBus;
using Architecture.Application.EventBus.Outbox;
using Architecture.Application.UnitOfWork;

namespace Architecture.Tests.Application.EventBus.Outbox;

public class EventOutboxFactoryTests
{
    [Fact]
    public void 應該取得最後一筆實作IEventOutbox且非EventOutbox的註冊物件()
    {
        // Given
        var unitOfWork = new Mock<IUnitOfWork>();
        var repository = new Mock<IIntegrationEventRepository>();
        var eventOutBox = new EventOutbox(unitOfWork.Object, repository.Object);
        var secondEventOutbox = new Mock<IEventOutbox>();
        var lastEventOutbox = new Mock<IEventOutbox>();
        var eventOutboxes = new List<IEventOutbox> { eventOutBox, secondEventOutbox.Object, lastEventOutbox.Object };
        var eventOutboxFactory = new EventOutboxFactory(eventOutboxes);

        // When
        var realityEventOutbox = eventOutboxFactory.GetRealityEventOutbox();

        // Then
        realityEventOutbox.Should().Be(lastEventOutbox.Object);
    }

    [Fact]
    public void 如果找不到最後一筆實作IEventOutbox且非EventOutbox的註冊物件_應該拋出InvalidOperationException()
    {
        // Given
        var unitOfWork = new Mock<IUnitOfWork>();
        var repository = new Mock<IIntegrationEventRepository>();
        var eventOutBox = new EventOutbox(unitOfWork.Object, repository.Object);
        var eventOutboxes = new List<IEventOutbox> { eventOutBox };
        var eventOutboxFactory = new EventOutboxFactory(eventOutboxes);

        // When
        var func = () => eventOutboxFactory.GetRealityEventOutbox();

        // Then
        func.Should().Throw<InvalidOperationException>();
    }
}
