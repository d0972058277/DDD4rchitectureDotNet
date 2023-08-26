using Architecture;
using Architecture.Core;
using Architecture.Shell.EventBus;

namespace Project.Application;

public abstract class IntegrationEventBase : IIntegrationEvent
{
    protected IntegrationEventBase()
    {
        Id = IdGenerator.NewId();
        CreationTimestamp = SystemDateTime.UtcNow;
    }

    public Guid Id { get; }

    public DateTime CreationTimestamp { get; }
}
