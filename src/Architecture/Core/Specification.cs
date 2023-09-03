using CSharpFunctionalExtensions;

namespace Architecture.Core;

public abstract class Specification<T>
{
    public Result<T> IsSatisfiedBy(T entity)
    {
        return Validate()(entity);
    }

    protected abstract Func<T, Result<T>> Validate();
}