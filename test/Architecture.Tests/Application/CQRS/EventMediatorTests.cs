using Architecture.Application.CQRS;
using MediatR;

namespace Architecture.Tests.Application.CQRS
{
    public partial class EventMediatorTests
    {
        [Fact]
        public async Task DispatchAsync的行為_應該使用Mediator的Send()
        {
            // Given
            var mediator = new Mock<IMediator>();
            var eventMediator = new EventMediator(mediator.Object);
            var domainEvent = new SomethingDomainEvent();

            // When
            await eventMediator.PublishAsync(domainEvent);

            // Then
            mediator.Verify(m => m.Publish(domainEvent, default), Times.Once());
        }

        [Fact]
        public async Task 無回應的ExecuteAsync的行為_應該使用Mediator的Send_TRequest()
        {
            // Given
            var mediator = new Mock<IMediator>();
            var eventMediator = new EventMediator(mediator.Object);
            var command = new SomethingCommand();

            // When
            await eventMediator.ExecuteAsync(command);

            // Then
            mediator.Verify(m => m.Send(command, default), Times.Once());
        }

        [Fact]
        public async Task 有回應的ExecuteAsync的行為_應該使用Mediator的Send_TResult()
        {
            // Given
            var mediator = new Mock<IMediator>();
            var eventMediator = new EventMediator(mediator.Object);
            var command = new SomethingBooleanCommand();

            // When
            await eventMediator.ExecuteAsync(command);

            // Then
            mediator.Verify(m => m.Send(command, default), Times.Once());
        }

        [Fact]
        public async Task PublishAsync的行為_應該使用Mediator的Publish()
        {
            // Given
            var mediator = new Mock<IMediator>();
            var eventMediator = new EventMediator(mediator.Object);
            var query = new SomethingBooleanQuery();

            // When
            await eventMediator.DispatchAsync(query);

            // Then
            mediator.Verify(m => m.Send(query, default), Times.Once());
        }
    }
}