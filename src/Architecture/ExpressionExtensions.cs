using System.Linq.Expressions;

namespace Architecture;

public static class ExpressionExtensions
{
    public static string GetPropertyName<T, U>(this Expression<Func<T, U>> propertyExpression)
    {
        if (propertyExpression.Body is not MemberExpression expression)
        {
            throw new ArgumentException(propertyExpression.Body.ToString());
        }

        return expression.Member.Name;
    }
}
