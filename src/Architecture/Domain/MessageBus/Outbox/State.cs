namespace Architecture.Domain.MessageBus.Outbox;

public enum State
{
    Raised = 0,
    InProgress = 1,
    Published = 2
}
