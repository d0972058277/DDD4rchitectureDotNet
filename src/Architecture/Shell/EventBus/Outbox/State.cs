namespace Architecture.Shell.EventBus.Outbox;

public enum State
{
    Raised = 0,
    InProgress = 1,
    Published = 2
}