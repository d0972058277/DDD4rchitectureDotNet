using Architecture.Domain;
using Project.Domain.SomethingContext.Events;

namespace Project.Domain.SomethingContext.Models;

public class SomethingAggregate : AggregateRoot<Guid>
{
    private SomethingAggregate(Guid id, SomethingEntity entity, List<SomethingValueObject> valueObjects) : base(id)
    {
        Entity = entity;
        _valueObjects = valueObjects;
    }

    public SomethingEntity Entity { get; private set; }

    private readonly List<SomethingValueObject> _valueObjects;
    public IReadOnlyList<SomethingValueObject> ValueObjects => _valueObjects.AsReadOnly();

    public static SomethingAggregate Create(Guid id, SomethingEntity entity, List<SomethingValueObject> valueObjects)
    {
        var aggregate = new SomethingAggregate(id, entity, valueObjects);
        aggregate.AddDomainEvent(new AggregateCreatedDomainEvent(aggregate.Id));
        return aggregate;
    }

    public void RenameEntity(string name)
    {
        Entity.Rename(name);
        AddDomainEvent(new EntityRenamedDomainEvent(Id));
    }
}
