using Architecture;
using Architecture.Domain.EventBus;

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
