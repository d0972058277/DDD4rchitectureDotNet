using Architecture.Domain;

namespace Architecture.Application.CQRS
{
    public static class EventMediatorExtensions
    {
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