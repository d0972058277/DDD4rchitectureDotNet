using System.Linq.Expressions;

namespace Architecture.Core;

public static class Selector<TSource>
{
    public static ISelector<TSource, TDestination> Set<TDestination>(Expression<Func<TSource, TDestination>> propertySelector)
        => new Selector<TSource, TDestination>(propertySelector);

    public static ISelector<TSource, TDestination> Set<TProperty, TDestination>(Expression<Func<TSource, TProperty>> propertySelector, Func<TProperty, TDestination> factory)
        => new Selector<TSource, TProperty, TDestination>(propertySelector, factory);
}

internal class Selector<TSource, TDestination> : ISelector<TSource, TDestination>
{
    private readonly Func<TSource, TDestination> _factory;

    internal Selector(Expression<Func<TSource, TDestination>> propertySelector)
    {
        _factory = propertySelector.Compile();
        PropertyName = propertySelector.GetPropertyName();
    }

    public string PropertyName { get; }

    public TDestination GetValue(TSource source) => _factory(source);
}

internal class Selector<TSource, TProperty, TDestination> : ISelector<TSource, TDestination>
{
    private readonly Func<TSource, TDestination> _factory;

    internal Selector(Expression<Func<TSource, TProperty>> propertySelector, Func<TProperty, TDestination> factory)
    {
        _factory = source => factory(propertySelector.Compile()(source));
        PropertyName = propertySelector.GetPropertyName();
    }

    public string PropertyName { get; }

    public TDestination GetValue(TSource source) => _factory(source);
}
