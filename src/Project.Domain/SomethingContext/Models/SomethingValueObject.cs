using Architecture;
using CSharpFunctionalExtensions;

namespace Project.Domain.SomethingContext.Models;

public class SomethingValueObject : ValueObject
{
    private SomethingValueObject(string @string, int number, bool boolean, DateTime dateTime)
    {
        String = @string;
        Number = number;
        Boolean = boolean;
        DateTime = dateTime;
    }

    public string String { get; }
    public int Number { get; }
    public bool Boolean { get; }
    public DateTime DateTime { get; }

    protected override IEnumerable<IComparable> GetEqualityComponents()
    {
        yield return String;
        yield return Number;
        yield return Boolean;
        yield return DateTime;
    }

    public static Result<SomethingValueObject> Create(string @string, int number, bool boolean, DateTime dateTime)
    {
        if (string.IsNullOrWhiteSpace(@string))
            return Result.Failure<SomethingValueObject>("String 不可為空");

        if (number < 0)
            return Result.Failure<SomethingValueObject>("Number 不可小於 0");

        if (!boolean)
            return Result.Failure<SomethingValueObject>("Boolean 不可為 False");

        if (dateTime < SystemDateTime.UtcNow)
            return Result.Failure<SomethingValueObject>("DateTime 不可小於系統的現在時間(UTC)");

        return new SomethingValueObject(@string, number, boolean, dateTime);
    }
}
