using Architecture.Core;
using Architecture.Shell.CQRS;

namespace Architecture.Tests.Shell.CQRS
{
    public partial class MediatorTests
    {
        public class SomethingBooleanQuery : IQuery<bool> { }
        public class SomethingCommand : ICommand { }
        public class SomethingBooleanCommand : ICommand<bool> { }
        public class SomethingDomainEvent : IDomainEvent { }

        [Fact]
        public async Task FetchAsync的行為_應該使用Mediator的Send()
        {
            // Given
            var mediatr = new Mock<MediatR.IMediator>();
            var mediator = new Mediator(mediatr.Object);
            var query = new SomethingBooleanQuery();

            // When
            await mediator.FetchAsync(query);

            // Then
            mediatr.Verify(m => m.Send(query, default), Times.Once());
        }

        [Fact]
        public async Task 無回應的ExecuteAsync的行為_應該使用Mediator的Send_TRequest()
        {
            // Given
            var mediatr = new Mock<MediatR.IMediator>();
            var mediator = new Mediator(mediatr.Object);
            var command = new SomethingCommand();

            // When
            await mediator.ExecuteAsync(command);

            // Then
            mediatr.Verify(m => m.Send(command, default), Times.Once());
        }

        [Fact]
        public async Task 有回應的ExecuteAsync的行為_應該使用Mediator的Send_TResult()
        {
            // Given
            var mediatr = new Mock<MediatR.IMediator>();
            var mediator = new Mediator(mediatr.Object);
            var command = new SomethingBooleanCommand();

            // When
            await mediator.ExecuteAsync(command);

            // Then
            mediatr.Verify(m => m.Send(command, default), Times.Once());
        }

        [Fact]
        public async Task PublishAsync的行為_應該使用Mediator的Publish()
        {
            // Given
            var mediatr = new Mock<MediatR.IMediator>();
            var mediator = new Mediator(mediatr.Object);
            var domainEvent = new SomethingDomainEvent();

            // When
            await mediator.PublishAsync(domainEvent);

            // Then
            mediatr.Verify(m => m.Publish(domainEvent, default), Times.Once());
        }
    }
}