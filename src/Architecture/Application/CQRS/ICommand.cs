using MediatR;

namespace Architecture.Application.CQRS
{
    public interface ICommand<out TResult> : IRequest<TResult> { }
    public interface ICommand : IRequest { }
}