using System.Linq.Expressions;

namespace Architecture.Core;

public static class ExpressionExtensions
{
    public static string GetPropertyName<TProperty, TName>(this Expression<Func<TProperty, TName>> propertyExpression)
    {
        if (propertyExpression.Body is not MemberExpression expression)
        {
            throw new ArgumentException(propertyExpression.Body.ToString());
        }

        return expression.Member.Name;
    }
}
