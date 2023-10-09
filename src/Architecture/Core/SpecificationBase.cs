using CSharpFunctionalExtensions;

namespace Architecture.Core;

public abstract class SpecificationBase<T>
{
    private IReadOnlyList<SpecificationRule<T>>? _specificationRules;

    protected abstract IEnumerable<SpecificationRule<T>> GetSpecificationRules();

    public Result<T> IsSatisfiedBy(T entity)
    {
        foreach (var rule in GetRules())
        {
            if (!rule.Validate(entity))
            {
                return Result.Failure<T>(rule.Message);
            }
        }

        return entity;
    }

    public IReadOnlyList<SpecificationRule<T>> GetRules()
    {
        _specificationRules ??= GetSpecificationRules().ToList();
        return _specificationRules;
    }
}