using Architecture.Application.CQRS;

namespace Project.Application.SomethingContext.Commands.RenameEntity;

public class RenameEntityCommand : ICommand
{
    public RenameEntityCommand(Guid somethingAggregateId, string entityName)
    {
        SomethingAggregateId = somethingAggregateId;
        EntityName = entityName;
    }

    public Guid SomethingAggregateId { get; }
    public string EntityName { get; }
}
