using Architecture.Core;
using Architecture.Shell.EventBus;

namespace Architecture.Tests;

public class SomethingIntegrationEvent : IIntegrationEvent
{
    public Guid Id { get; init; } = IdGenerator.NewId();
    public DateTime CreationTimestamp { get; init; } = SystemDateTime.UtcNow;
}
