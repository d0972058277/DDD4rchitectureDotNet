namespace Architecture.Application.EventBus;

public interface IEventPublisherFactory
{
    IEventPublisher GetOutboxEventPublisher();
    IEventPublisher GetRealityEventPublisher();
}
