using Architecture.Shell.CQRS;
using Project.Domain.SomethingContext.Models;

namespace Project.Application.SomethingContext.Commands.CreateAggregate;

public class CreateAggregateCommand : ICommand<Guid>
{
    public CreateAggregateCommand(string entityName, List<SomethingValueObject> valueObjects)
    {
        EntityName = entityName;
        ValueObjects = valueObjects;
    }

    public string EntityName { get; }
    public List<SomethingValueObject> ValueObjects { get; }
}
