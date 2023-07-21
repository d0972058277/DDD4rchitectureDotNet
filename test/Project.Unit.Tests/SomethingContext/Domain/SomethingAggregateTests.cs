using Architecture;
using FluentAssertions;
using Project.Domain.SomethingContext.Events;
using Project.Domain.SomethingContext.Models;

namespace Project.Unit.Tests.SomethingContext.Domain;

public class SomethingAggregateTests
{
    [Fact]
    public void 帶入正確的參數_應該能夠成功建立()
    {
        // Given
        var id = IdGenerator.NewId();
        var entity = GetSomethingEntity();
        var valueObjects = GetSomethingValueObjects();

        // When
        var aggregate = SomethingAggregate.Create(id, entity, valueObjects);

        // Then
        aggregate.Id.Should().Be(id);
        aggregate.Entity.Should().Be(entity);
        aggregate.ValueObjects.Should().BeEquivalentTo(valueObjects);
        aggregate.DomainEvents.Single().As<AggregateCreatedDomainEvent>().SomethingAggregateId.Should().Be(aggregate.Id);
    }

    [Fact]
    public void 應該能夠重新命名Entity()
    {
        // Given
        var aggregate = GetSomethingAggregate();
        var name = "newName";

        // When
        aggregate.RenameEntity(name);

        // Then
        aggregate.Entity.Name.Should().Be(name);
        aggregate.DomainEvents.Single().As<EntityRenamedDomainEvent>().SomethingAggregateId.Should().Be(aggregate.Id);
    }

    private static SomethingAggregate GetSomethingAggregate()
    {
        var id = IdGenerator.NewId();
        var entity = GetSomethingEntity();
        var valueObjects = GetSomethingValueObjects();
        var aggregate = SomethingAggregate.Create(id, entity, valueObjects);
        aggregate.ClearDomainEvents();
        return aggregate;
    }

    private static SomethingEntity GetSomethingEntity()
    {
        var id = IdGenerator.NewId();
        var name = "name";
        var entity = SomethingEntity.Create(id, name);
        return entity;
    }

    private static List<SomethingValueObject> GetSomethingValueObjects()
    {
        return Enumerable.Range(1, 10).Select(i =>
        {
            var @string = $"string{i}";
            var number = i;
            var boolean = true;
            var dateTime = SystemDateTime.UtcNow.AddSeconds(1);

            return SomethingValueObject.Create(@string, number, boolean, dateTime).Value;
        }).ToList();
    }
}
