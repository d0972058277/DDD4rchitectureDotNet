namespace Architecture.Core;

public interface ISelector<in TSource, out TDestination>
{
    string PropertyName { get; }
    TDestination GetValue(TSource source);
}
