using Architecture.Core;

namespace Architecture.Shell.CQRS
{
    public class Mediator : IMediator
    {
        private readonly MediatR.IMediator _mediator;

        public Mediator(MediatR.IMediator mediator)
        {
            _mediator = mediator;
        }

        public Task<TResult> FetchAsync<TResult>(IQuery<TResult> query, CancellationToken cancellationToken = default)
        {
            return _mediator.Send(query, cancellationToken);
        }

        public Task ExecuteAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default) where TCommand : ICommand
        {
            return _mediator.Send(command, cancellationToken);
        }

        public Task<TResult> ExecuteAsync<TResult>(ICommand<TResult> command, CancellationToken cancellationToken = default)
        {
            return _mediator.Send(command, cancellationToken);
        }

        public Task PublishAsync<TDomainEvent>(TDomainEvent domainEvent, CancellationToken cancellationToken = default) where TDomainEvent : IDomainEvent
        {
            return _mediator.Publish(domainEvent, cancellationToken);
        }
    }
}