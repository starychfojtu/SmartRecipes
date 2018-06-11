using System;
using System.Linq.Expressions;
using LanguageExt;
using System.Threading.Tasks;

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

        // TODO: Generalize this !!
        public static Reader<E, Task<B>> Bind<E, A, B>(this Reader<E, Task<A>> reader, Func<A, Reader<E, Task<B>>> binder)
        {
            return env =>
            {
                var (aTask, aIsFaulted) = reader(env);
                var bTask = aTask.Bind(a => binder(a)(env).Value);
                return (bTask, false);
            };
        }
    }
}
