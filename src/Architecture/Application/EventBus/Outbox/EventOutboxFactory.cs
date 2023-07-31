namespace Architecture.Application.EventBus.Outbox;

public class EventOutboxFactory : IEventOutboxFactory
{
    private readonly IEnumerable<IEventOutbox> _eventOutboxes;

    public EventOutboxFactory(IEnumerable<IEventOutbox> eventOutboxes)
    {
        _eventOutboxes = eventOutboxes;
    }

    public IEventOutbox GetRealityEventOutbox()
    {
        var eventOutbox = _eventOutboxes.Where(b => b is not EventOutbox).LastOrDefault();

        return eventOutbox is null ?
            throw new InvalidOperationException($"未實作或未註冊實際進行事件發佈的 {nameof(IEventOutbox)} 類別") :
            eventOutbox;
    }
}
