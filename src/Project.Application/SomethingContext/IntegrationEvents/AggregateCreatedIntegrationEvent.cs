using Architecture;
using Architecture.Core;
using Architecture.Shell.EventBus;

namespace Project.Application.SomethingContext.IntegrationEvents;

public class AggregateCreatedIntegrationEvent : IIntegrationEvent
{
    public Guid Id { get; init; } = IdGenerator.NewId();
    public DateTime CreationTimestamp { get; init; } = SystemDateTime.UtcNow;
    public Guid SomethingAggregateId { get; init; }
}
