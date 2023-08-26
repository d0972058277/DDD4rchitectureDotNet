using Architecture.Shell.CQRS;
using Project.Application.SomethingContext.Repositories;

namespace Project.Application.SomethingContext.Commands.RenameEntity;

public class RenameEntityCommandHandler : ICommandHandler<RenameEntityCommand>
{
    private readonly ISomethingRepository _repository;
    private readonly IMediator _mediator;

    public RenameEntityCommandHandler(ISomethingRepository repository, IMediator mediator)
    {
        _repository = repository;
        _mediator = mediator;
    }

    public async Task Handle(RenameEntityCommand request, CancellationToken cancellationToken)
    {
        var aggregate = await _repository.FindAsync(request.SomethingAggregateId, cancellationToken);
        aggregate.RenameEntity(request.EntityName);
        await _repository.SaveAsync(aggregate, cancellationToken);
        await _mediator.PublishAndClearDomainEvents(aggregate, cancellationToken);
    }
}
