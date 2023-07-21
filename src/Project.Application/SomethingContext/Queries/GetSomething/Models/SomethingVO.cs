using Project.Domain.SomethingContext.Models;

namespace Project.Application.SomethingContext.Queries.GetSomething.Models;

public class SomethingVO
{
    private SomethingVO(string @string, int number, bool boolean, DateTime dateTime)
    {
        String = @string;
        Number = number;
        Boolean = boolean;
        DateTime = dateTime;
    }

    public string String { get; } = string.Empty;
    public int Number { get; }
    public bool Boolean { get; }
    public DateTime DateTime { get; }

    public static SomethingVO Create(SomethingValueObject obj)
    {
        return new SomethingVO(obj.String, obj.Number, obj.Boolean, obj.DateTime);
    }

    public static IReadOnlyList<SomethingVO> Create(IEnumerable<SomethingValueObject> objs)
    {
        return objs.Select(obj => new SomethingVO(obj.String, obj.Number, obj.Boolean, obj.DateTime)).ToList();
    }
}
