namespace Architecture.Application.EventBus.Outbox;

public class EventPublisherFactory : IEventPublisherFactory
{
    private readonly IEnumerable<IEventPublisher> _eventPublisheres;

    public EventPublisherFactory(IEnumerable<IEventPublisher> eventPublisheres)
    {
        _eventPublisheres = eventPublisheres;
    }

    public IEventPublisher GetOutboxEventPublisher()
    {
        var eventPublisher = _eventPublisheres.SingleOrDefault(p => p is OutboxEventPublisher);

        return eventPublisher is null ?
            throw new InvalidOperationException($"未註冊 {nameof(IEventPublisher)} 的 ${nameof(OutboxEventPublisher)} 實作類別") :
            eventPublisher;
    }

    public IEventPublisher GetRealityEventPublisher()
    {
        var eventPublisher = _eventPublisheres.Where(p => p is not OutboxEventPublisher).LastOrDefault();

        return eventPublisher is null ?
            throw new InvalidOperationException($"未實作或未註冊實際進行事件發佈的 {nameof(IEventPublisher)} 類別") :
            eventPublisher;
    }
}
