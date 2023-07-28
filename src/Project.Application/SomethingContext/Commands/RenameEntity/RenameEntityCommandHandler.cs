using Architecture.Application.CQRS;
using Project.Application.SomethingContext.Repositories;

namespace Project.Application.SomethingContext.Commands.RenameEntity;

public class RenameEntityCommandHandler : ICommandHandler<RenameEntityCommand>
{
    private readonly ISomethingRepository _repository;
    private readonly IEventMediator _eventMediator;

    public RenameEntityCommandHandler(ISomethingRepository repository, IEventMediator eventMediator)
    {
        _repository = repository;
        _eventMediator = eventMediator;
    }

    public async Task Handle(RenameEntityCommand request, CancellationToken cancellationToken)
    {
        var aggregate = await _repository.FindAsync(request.SomethingAggregateId, cancellationToken);
        aggregate.RenameEntity(request.EntityName);
        await _repository.SaveAsync(aggregate, cancellationToken);
        await _eventMediator.PublishAndClearDomainEvents(aggregate, cancellationToken);
    }
}
