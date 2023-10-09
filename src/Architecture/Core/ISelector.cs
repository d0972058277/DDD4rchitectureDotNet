namespace Architecture.Core;

public interface ISelector<TSource, TDestination>
{
    string PropertyName { get; }
    TDestination GetValue(TSource source);
}
