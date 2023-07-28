using Architecture;
using Architecture.Application.CQRS;
using FluentAssertions;
using Project.Application.SomethingContext.Commands.CreateAggregate;
using Project.Application.SomethingContext.Queries.GetSomething;
using Project.Domain.SomethingContext.Models;
using Project.Infrastructure;

namespace Project.Integration.Tests.UseCase.SomethingContext;

[Collection("Sequential")]
public class CreateAggregateUseCase
{
    private readonly ProjectDbContext _dbContext;
    private readonly IEventMediator _eventMediator;

    public CreateAggregateUseCase(ProjectDbContext dbContext, IEventMediator eventMediator)
    {
        _dbContext = dbContext;
        _eventMediator = eventMediator;
    }

    [Fact]
    public async Task 應該可以創建Aggregate並查詢到()
    {
        await _dbContext.ResetDatabaseAsync();

        // Given
        var entityName = "entityName";
        var valueObjects = GetSomethingValueObjects();

        var command = new CreateAggregateCommand(entityName, valueObjects);
        var aggregateId = await _eventMediator.ExecuteAsync(command);

        // When
        var query = new GetSomethingQuery(aggregateId);
        var something = await _eventMediator.DispatchAsync(query);

        // Then
        something.Id.Should().Be(aggregateId);
        something.EntityName.Should().Be(entityName);
        something.ValueObjects.Select(o => o.String).Should().BeEquivalentTo(valueObjects.Select(o => o.String));
        something.ValueObjects.Select(o => o.Number).Should().BeEquivalentTo(valueObjects.Select(o => o.Number));
        something.ValueObjects.Select(o => o.Boolean).Should().BeEquivalentTo(valueObjects.Select(o => o.Boolean));
        something.ValueObjects.Select(o => o.DateTime).Should().BeEquivalentTo(valueObjects.Select(o => o.DateTime));
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
