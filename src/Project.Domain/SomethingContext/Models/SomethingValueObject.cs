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
            Selector<SomethingValueObject>.Set(x => x.String),
            Selector<SomethingValueObject>.Set(x => x.Number),
            Selector<SomethingValueObject>.Set(x => x.Boolean),
            Selector<SomethingValueObject>.Set(x => x.DateTime));
        var instance = new SomethingValueObject(@string, number, boolean, dateTime);
        return specification.IsSatisfiedBy(instance);
    }

    public class Specification<T> : SpecificationBase<T>
    {
        private readonly ISelector<T, string> _stringSelector;
        private readonly ISelector<T, int> _numberSelector;
        private readonly ISelector<T, bool> _booleanSelector;
        private readonly ISelector<T, DateTime> _dateTimeSelector;

        private Specification(ISelector<T, string> stringSelector, ISelector<T, int> numberSelector, ISelector<T, bool> booleanSelector, ISelector<T, DateTime> dateTimeSelector)
        {
            _stringSelector = stringSelector;
            _numberSelector = numberSelector;
            _booleanSelector = booleanSelector;
            _dateTimeSelector = dateTimeSelector;
        }

        public override IEnumerable<SpecificationRule<T>> GetRules()
        {
            yield return new SpecificationRule<T>($"{_stringSelector.PropertyName} 應該不可為空或空字串", arg => !string.IsNullOrWhiteSpace(_stringSelector.GetValue(arg)));
            yield return new SpecificationRule<T>($"{_numberSelector.PropertyName} 應該大於等於 0", arg => _numberSelector.GetValue(arg) >= 0);
            yield return new SpecificationRule<T>($"{_booleanSelector.PropertyName} 應該為 True", arg => _booleanSelector.GetValue(arg));
            yield return new SpecificationRule<T>($"{_dateTimeSelector.PropertyName} 應該大於等於系統的現在時間(UTC)", arg => _dateTimeSelector.GetValue(arg) >= SystemDateTime.UtcNow);
        }

        public static Specification<T> Create(ISelector<T, string> stringSelector, ISelector<T, int> numberSelector, ISelector<T, bool> booleanSelector, ISelector<T, DateTime> dateTimeSelector)
        {
            return new Specification<T>(stringSelector, numberSelector, booleanSelector, dateTimeSelector);
        }
    }
}
