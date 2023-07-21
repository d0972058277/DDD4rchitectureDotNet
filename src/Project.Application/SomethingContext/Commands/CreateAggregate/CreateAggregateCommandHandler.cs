using Architecture;
using Architecture.Application.CQRS;
using Project.Application.SomethingContext.Repositories;
using Project.Domain.SomethingContext.Models;

namespace Project.Application.SomethingContext.Commands.CreateAggregate;

public class CreateAggregateCommandHandler : ICommandHandler<CreateAggregateCommand, Guid>
{
    private readonly ISomethingRepository _repository;
    private readonly IEventMediator _eventMediator;

    public CreateAggregateCommandHandler(ISomethingRepository repository, IEventMediator eventMediator)
    {
        _repository = repository;
        _eventMediator = eventMediator;
    }

    public async Task<Guid> Handle(CreateAggregateCommand request, CancellationToken cancellationToken)
    {
        var aggregateId = IdGenerator.NewId();
        var entityId = IdGenerator.NewId();
        var entity = SomethingEntity.Create(entityId, request.EntityName);
        var aggregate = SomethingAggregate.Create(aggregateId, entity, request.ValueObjects);
        await _repository.AddAsync(aggregate, cancellationToken);
        await _eventMediator.PublishAndClearDomainEvents(aggregate, cancellationToken);
        return aggregate.Id;
    }
}
