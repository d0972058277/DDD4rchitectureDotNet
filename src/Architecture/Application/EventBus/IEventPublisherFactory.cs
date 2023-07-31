namespace Architecture.Application.EventBus;

public interface IEventPublisherFactory
{
    IEventPublisher GetRealityEventPublisher();
}
