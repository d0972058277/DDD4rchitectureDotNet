using Architecture.Core;
using Architecture.Shell.CQRS;
using Project.Application.SomethingContext.Repositories;
using Project.Domain.SomethingContext.Models;

namespace Project.Application.SomethingContext.Commands.CreateAggregate;

public class CreateAggregateCommandHandler : ICommandHandler<CreateAggregateCommand, Guid>
{
    private readonly ISomethingRepository _repository;
    private readonly IMediator _mediator;

    public CreateAggregateCommandHandler(ISomethingRepository repository, IMediator mediator)
    {
        _repository = repository;
        _mediator = mediator;
    }

    public async Task<Guid> Handle(CreateAggregateCommand request, CancellationToken cancellationToken)
    {
        var aggregateId = IdGenerator.NewId();
        var entityId = IdGenerator.NewId();
        var entity = SomethingEntity.Create(entityId, request.EntityName);
        var aggregate = SomethingAggregate.Create(aggregateId, entity, request.ValueObjects);
        await _repository.AddAsync(aggregate, cancellationToken);
        await _mediator.PublishAndClearDomainEvents(aggregate, cancellationToken);
        return aggregate.Id;
    }
}
