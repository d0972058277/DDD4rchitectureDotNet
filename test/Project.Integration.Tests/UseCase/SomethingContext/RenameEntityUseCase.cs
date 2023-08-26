using Architecture;
using Architecture.Core;
using Architecture.Shell.CQRS;
using FluentAssertions;
using Project.Application.SomethingContext.Commands.RenameEntity;
using Project.Application.SomethingContext.Queries.GetSomething;
using Project.Domain.SomethingContext.Models;
using Project.Infrastructure;

namespace Project.Integration.Tests.UseCase.SomethingContext;

[Collection("Sequential")]
public class RenameEntityUseCase
{
    private readonly ProjectDbContext _dbContext;
    private readonly IMediator _mediator;

    public RenameEntityUseCase(ProjectDbContext dbContext, IMediator mediator)
    {
        _dbContext = dbContext;
        _mediator = mediator;
    }

    [Fact]
    public async Task 應該可以重新命名Entity的名稱並查詢到()
    {
        await _dbContext.ResetDatabaseAsync();
        var aggregate = GetSomethingAggregate();
        await _dbContext.InitDataAsync(aggregate);

        // Given
        var entityName = "entityName";
        var command = new RenameEntityCommand(aggregate.Id, entityName);
        await _mediator.ExecuteAsync(command);

        // When
        var query = new GetSomethingQuery(aggregate.Id);
        var something = await _mediator.FetchAsync(query);

        // Then
        something.EntityName.Should().Be(entityName);
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
