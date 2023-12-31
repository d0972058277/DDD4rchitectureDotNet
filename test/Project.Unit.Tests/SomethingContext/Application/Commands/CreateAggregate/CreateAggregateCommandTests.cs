using Architecture;
using Architecture.Core;
using Architecture.Shell.CQRS;
using Moq;
using Project.Application.SomethingContext.Commands.CreateAggregate;
using Project.Application.SomethingContext.Repositories;
using Project.Domain.SomethingContext.Events;
using Project.Domain.SomethingContext.Models;

namespace Project.Unit.Tests.SomethingContext.Application.Commands.CreateAggregate;

public class CreateAggregateCommandTests
{
    [Fact]
    public async Task 帶入正確的參數_應該能夠成功建立()
    {
        // Given
        var entityName = "entityName";
        var valueObjects = GetSomethingValueObjects();

        var repository = new Mock<ISomethingRepository>();
        var mediator = new Mock<IMediator>();

        var command = new CreateAggregateCommand(entityName, valueObjects);
        var handler = new CreateAggregateCommandHandler(repository.Object, mediator.Object);

        // When
        var aggregateId = await handler.Handle(command, default);

        // Then
        repository.Verify(m => m.AddAsync(It.Is<SomethingAggregate>(a => a.Id == aggregateId), default), Times.Once());
        mediator.Verify(m => m.PublishAsync(It.Is<IDomainEvent>(e => e is AggregateCreatedDomainEvent), default), Times.Once());
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
