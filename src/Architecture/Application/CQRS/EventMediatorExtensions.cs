using Architecture.Domain;

namespace Architecture.Application.CQRS
{
    public static class EventMediatorExtensions
    {
        public static async Task PublishAndClearDomainEvents(this IEventMediator mediator, IAggregateRoot aggregateRoot, CancellationToken cancellationToken = default)
        {
            // NOTE: 由於是 call by reference ，如果不透過 ToList 複製，下一段 ClearDomainEvents 會將 DomainEvents 清除
            var domainEvents = aggregateRoot.DomainEvents.ToList();

            aggregateRoot.ClearDomainEvents();

            await mediator.PublishAsync(domainEvents, cancellationToken);
        }

        public static async Task PublishAsync(this IEventMediator mediator, IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken = default)
        {
            foreach (var domainEvent in domainEvents)
            {
                var domainEventType = domainEvent.GetType();

                var publishMethod = typeof(IEventMediator)
                    .GetMethod(nameof(IEventMediator.PublishAsync))!
                    .MakeGenericMethod(domainEventType);

                await (Task)publishMethod.Invoke(mediator, new object[] { domainEvent, cancellationToken })!;
            }
        }
    }
}