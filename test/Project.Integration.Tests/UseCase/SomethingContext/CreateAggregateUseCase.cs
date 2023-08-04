using Architecture;
using Architecture.Application.CQRS;
using FluentAssertions;
using MassTransit.Testing;
using Microsoft.EntityFrameworkCore;
using Project.Application.SomethingContext.Commands.CreateAggregate;
using Project.Application.SomethingContext.IntegrationEvents;
using Project.Application.SomethingContext.Queries.GetSomething;
using Project.Domain.SomethingContext.Models;
using Project.Infrastructure;

namespace Project.Integration.Tests.UseCase.SomethingContext;

[Collection("Sequential")]
public class CreateAggregateUseCase
{
    private readonly ProjectDbContext _dbContext;
    private readonly ReadOnlyProjectDbContext _readOnlyDbContext;
    private readonly IEventMediator _eventMediator;
    private readonly ITestHarness _testHarness;

    public CreateAggregateUseCase(ProjectDbContext dbContext, ReadOnlyProjectDbContext readOnlyDbContext, IEventMediator eventMediator, ITestHarness testHarness)
    {
        _dbContext = dbContext;
        _readOnlyDbContext = readOnlyDbContext;
        _eventMediator = eventMediator;
        _testHarness = testHarness;
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

        var integrationEventEntry = await _readOnlyDbContext.Outbox.SingleAsync();
        integrationEventEntry.GetPayload().Deserialize().As<AggregateCreatedIntegrationEvent>().SomethingAggregateId.Should().Be(something.Id);

        await Task.Delay(1000);

        // NOTE: 驗證事件已發佈
        (await _testHarness.Published.Any<AggregateCreatedIntegrationEvent>(e => e.Context.Message.SomethingAggregateId == something.Id)).Should().BeTrue();

        // NOTE: 驗證事件已消耗
        (await _testHarness.Consumed.Any<AggregateCreatedIntegrationEvent>(e => e.Context.Message.SomethingAggregateId == something.Id)).Should().BeTrue();
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
