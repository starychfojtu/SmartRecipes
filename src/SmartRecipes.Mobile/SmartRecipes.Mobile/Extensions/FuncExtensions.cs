using System;
using System.Linq.Expressions;

namespace SmartRecipes.Mobile
{
    public static class FuncExtensions
    {
        public static string ToPropertyPathName<T, TProperty>(this Expression<Func<T, TProperty>> memberExpression)
        {
            // TODO: check performance of this
            //var expression = memberExpression.Body as MemberExpression;
            //var member = expression.Member as PropertyInfo;
            //return member.Name;

            var expressions = memberExpression.ToString();
            var indexOfFirstDot = expressions.IndexOf('.');
            var propertyPathName = expressions.Substring(indexOfFirstDot + 1);
            return propertyPathName;
        }
    }
}
