namespace Project.Application.SomethingContext.Queries.GetSomething.Models;

public class Something
{
    public Guid Id { get; }
    public string EntityName { get; }

    private readonly List<SomethingVO> _valueObjects;

    private Something(Guid id, string entityName, List<SomethingVO> valueObjects)
    {
        Id = id;
        EntityName = entityName;
        _valueObjects = valueObjects;
    }

    public IReadOnlyList<SomethingVO> ValueObjects => _valueObjects;

    public static Something Create(Guid id, string entityName, IEnumerable<SomethingVO> valueObject)
    {
        return new Something(id, entityName, valueObject.ToList());
    }
}
