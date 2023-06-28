using MediatR;

namespace Architecture.Application.CQRS
{
    public interface IBaseQuery { }
    public interface IQuery<out TResult> : IBaseQuery, IRequest<TResult> { }
}