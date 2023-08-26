using System.Collections.Concurrent;
using System.Reflection;
using Architecture.Core;

namespace Architecture.Shell.CQRS
{
    public static class MediatorExtensions
    {
        private static readonly ConcurrentDictionary<Type, MethodInfo> PublishMethodsCache = new();

        public static async Task PublishAsync(this IMediator mediator, IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken = default)
        {
            foreach (var domainEvent in domainEvents)
            {
                var domainEventType = domainEvent.GetType();
                var publishMethod = PublishMethodsCache.GetOrAdd(domainEventType, GetPublishMethod);
                await (Task)publishMethod.Invoke(mediator, new object[] { domainEvent, cancellationToken })!;
            }
        }

        private static MethodInfo GetPublishMethod(Type domainEventType)
        {
            var publishMethod = typeof(IMediator)
                .GetMethod(nameof(IMediator.PublishAsync))!
                .MakeGenericMethod(domainEventType);
            return publishMethod;
        }

        public static async Task PublishAndClearDomainEvents(this IMediator mediator, IAggregateRoot aggregateRoot, CancellationToken cancellationToken = default)
        {
            // NOTE: 由於是 call by reference ，如果不透過 ToList 複製，下一段 ClearDomainEvents 會將 DomainEvents 清除
            var domainEvents = aggregateRoot.DomainEvents.ToList();

            aggregateRoot.ClearDomainEvents();

            await mediator.PublishAsync(domainEvents, cancellationToken);
        }
    }
}