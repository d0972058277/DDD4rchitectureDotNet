using System.Linq.Expressions;
using Architecture;
using Architecture.Core;
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
        var specification = SomethingValueObjectSpecification<SomethingValueObject>.Create(
            x => x.String,
            x => x.Number,
            x => x.Boolean,
            x => x.DateTime);
        var instance = new SomethingValueObject(@string, number, boolean, dateTime);
        return specification.IsSatisfiedBy(instance);
    }

    public class SomethingValueObjectSpecification<T> : Specification<T>
    {
        private readonly Expression<Func<T, string>> _stringExpression;
        private readonly Expression<Func<T, int>> _numberExpression;
        private readonly Expression<Func<T, bool>> _booleanExpression;
        private readonly Expression<Func<T, DateTime>> _dateTimeExpression;

        private SomethingValueObjectSpecification(Expression<Func<T, string>> stringExpression, Expression<Func<T, int>> numberExpression, Expression<Func<T, bool>> booleanExpression, Expression<Func<T, DateTime>> dateTimeExpression)
        {
            _stringExpression = stringExpression;
            _numberExpression = numberExpression;
            _booleanExpression = booleanExpression;
            _dateTimeExpression = dateTimeExpression;
        }

        protected override Func<T, Result<T>> Validate() => arg =>
        {
            var @string = _stringExpression.Compile()(arg);
            if (string.IsNullOrWhiteSpace(@string))
                return Result.Failure<T>($"{_stringExpression.GetPropertyName()} 不可為空");

            var number = _numberExpression.Compile()(arg);
            if (number < 0)
                return Result.Failure<T>($"{_numberExpression.GetPropertyName()} 不可小於 0");

            var boolean = _booleanExpression.Compile()(arg);
            if (!boolean)
                return Result.Failure<T>($"{_booleanExpression.GetPropertyName()} 不可為 False");

            var dateTime = _dateTimeExpression.Compile()(arg);
            if (dateTime < SystemDateTime.UtcNow)
                return Result.Failure<T>($"{_dateTimeExpression.GetGenericTypeName()} 不可小於系統的現在時間(UTC)");

            return arg;
        };

        public static SomethingValueObjectSpecification<T> Create(Expression<Func<T, string>> stringExpression, Expression<Func<T, int>> numberExpression, Expression<Func<T, bool>> booleanExpression, Expression<Func<T, DateTime>> dateTimeExpression)
        {
            return new SomethingValueObjectSpecification<T>(stringExpression, numberExpression, booleanExpression, dateTimeExpression);
        }
    }
}
