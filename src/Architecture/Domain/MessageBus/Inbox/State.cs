namespace Architecture.Domain.MessageBus.Inbox;

public enum State
{
    Subscribed = 0,
    InProgress = 1,
    Consumed = 2
}
