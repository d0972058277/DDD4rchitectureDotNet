using Architecture;
using Architecture.Application.CQRS;
using Moq;
using Project.Application.SomethingContext.Commands.RenameEntity;
using Project.Application.SomethingContext.Repositories;
using Project.Domain.SomethingContext.Events;
using Project.Domain.SomethingContext.Models;

namespace Project.Unit.Tests.SomethingContext.Application.Commands.RenameEntity;

public class RenameEntityCommandTests
{
    [Fact]
    public async Task 應該能夠重新命名Entity()
    {
        // Given
        var aggregate = GetSomethingAggregate();
        var entityName = "entityName";

        var repository = new Mock<ISomethingRepository>();
        repository.Setup(m => m.FindAsync(aggregate.Id, default)).ReturnsAsync(aggregate);
        var eventMediator = new Mock<IEventMediator>();

        var command = new RenameEntityCommand(aggregate.Id, entityName);
        var handler = new RenameEntityCommandHandler(repository.Object, eventMediator.Object);

        // When
        await handler.Handle(command, default);

        // Then
        repository.Verify(m => m.SaveAsync(It.Is<SomethingAggregate>(a => a == aggregate), default), Times.Once());
        eventMediator.Verify(m => m.PublishAsync(It.Is<EntityRenamedDomainEvent>(e => e.SomethingAggregateId == aggregate.Id), default), Times.Once());
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
