using MediatR;

namespace Architecture.Shell.CQRS
{
    public interface IBaseQuery { }
    public interface IQuery<out TResult> : IBaseQuery, IRequest<TResult> { }
}