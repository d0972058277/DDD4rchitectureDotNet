namespace Architecture.Domain.MessageBus.Outbox;

public enum State
{
    NotPublished = 0,
    InProgress = 1,
    Published = 2,
    PublishedFailed = 3
}
