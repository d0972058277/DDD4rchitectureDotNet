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
        var specification = Specification<SomethingValueObject>.Create(
            x => x.String,
            x => x.Number,
            x => x.Boolean,
            x => x.DateTime);
        var instance = new SomethingValueObject(@string, number, boolean, dateTime);
        return specification.IsSatisfiedBy(instance);
    }

    public class Specification<T> : SpecificationBase<T>
    {
        private readonly Expression<Func<T, string>> _stringExpression;
        private readonly Expression<Func<T, int>> _numberExpression;
        private readonly Expression<Func<T, bool>> _booleanExpression;
        private readonly Expression<Func<T, DateTime>> _dateTimeExpression;

        private Specification(Expression<Func<T, string>> stringExpression, Expression<Func<T, int>> numberExpression, Expression<Func<T, bool>> booleanExpression, Expression<Func<T, DateTime>> dateTimeExpression)
        {
            _stringExpression = stringExpression;
            _numberExpression = numberExpression;
            _booleanExpression = booleanExpression;
            _dateTimeExpression = dateTimeExpression;
        }

        public override IEnumerable<SpecificationRule<T>> GetRules()
        {
            yield return new SpecificationRule<T>($"{_stringExpression.GetPropertyName()} 應該不可為空或空字串", arg => !string.IsNullOrWhiteSpace(_stringExpression.Compile()(arg)));
            yield return new SpecificationRule<T>($"{_numberExpression.GetPropertyName()} 應該大於等於 0", arg => _numberExpression.Compile()(arg) >= 0);
            yield return new SpecificationRule<T>($"{_booleanExpression.GetPropertyName()} 應該為 True", arg => _booleanExpression.Compile()(arg));
            yield return new SpecificationRule<T>($"{_dateTimeExpression.GetPropertyName()} 應該大於等於系統的現在時間(UTC)", arg => _dateTimeExpression.Compile()(arg) >= SystemDateTime.UtcNow);
        }

        public static Specification<T> Create(Expression<Func<T, string>> stringExpression, Expression<Func<T, int>> numberExpression, Expression<Func<T, bool>> booleanExpression, Expression<Func<T, DateTime>> dateTimeExpression)
        {
            return new Specification<T>(stringExpression, numberExpression, booleanExpression, dateTimeExpression);
        }
    }
}
