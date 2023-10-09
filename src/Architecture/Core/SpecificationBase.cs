using CSharpFunctionalExtensions;

namespace Architecture.Core;

public abstract class SpecificationBase<T>
{
    private IReadOnlyList<SpecificationRule<T>>? _specificationRules;

    private IReadOnlyList<SpecificationRule<T>> GetSpecificationRules()
    {
        _specificationRules ??= GetRules().ToList();
        return _specificationRules;
    }

    public Result<T> IsSatisfiedBy(T entity)
    {
        foreach (var rule in GetSpecificationRules())
        {
            if (!rule.Validate(entity))
            {
                return Result.Failure<T>(rule.Message);
            }
        }

        return entity;
    }

    public abstract IEnumerable<SpecificationRule<T>> GetRules();
}