namespace Architecture.Application.EventBus.Outbox;

public class EventPublisherFactory : IEventPublisherFactory
{
    private readonly IEnumerable<IEventPublisher> _eventPublisheres;

    public EventPublisherFactory(IEnumerable<IEventPublisher> eventPublisheres)
    {
        _eventPublisheres = eventPublisheres;
    }

    public IEventPublisher GetRealityEventPublisher()
    {
        var eventPublisher = _eventPublisheres.Where(b => b is not OutboxEventPublisher).LastOrDefault();

        return eventPublisher is null ?
            throw new InvalidOperationException($"未實作或未註冊實際進行事件發佈的 {nameof(IEventPublisher)} 類別") :
            eventPublisher;
    }
}
