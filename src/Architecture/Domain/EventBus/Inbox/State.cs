namespace Architecture.Domain.EventBus.Inbox;

public enum State
{
    Received = 0,
    InProgress = 1,
    Handled = 2
}
