namespace Architecture.Application.EventBus;

public interface IEventOutboxFactory
{
    IEventOutbox GetRealityEventOutbox();
}
