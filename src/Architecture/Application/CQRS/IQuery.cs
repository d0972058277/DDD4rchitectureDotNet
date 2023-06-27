using MediatR;

namespace Architecture.Application.CQRS
{
    public interface IQuery<out TResult> : IRequest<TResult> { }
}