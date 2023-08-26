using MediatR;

namespace Architecture.Shell.CQRS
{
    public interface IBaseCommand { }
    public interface ICommand<out TResult> : IBaseCommand, IRequest<TResult> { }
    public interface ICommand : IBaseCommand, IRequest { }
}