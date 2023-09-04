using CSharpFunctionalExtensions;

namespace Architecture.Core;

public abstract class SpecificationBase<T>
{
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

    public abstract IEnumerable<SpecificationRule<T>> GetRules();
}