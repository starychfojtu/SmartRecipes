using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SmartRecipes.Mobile.Extensions
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

        // TODO: Generalize this or find this function in API !!
        public static Monad.Reader<E, Task<B>> Bind<E, A, B>(this Monad.Reader<E, Task<A>> reader, Func<A, Monad.Reader<E, Task<B>>> binder)
        {
            return env => reader(env).Bind(a => binder(a)(env));
        }

        // TODO: Generalize this or find this function in API !!
        public static Monad.Reader<E, Task<C>> SelectMany<E, A, B, C>(
            this Monad.Reader<E, Task<A>> reader,
            Func<A, Monad.Reader<E, Task<B>>> binder,
            Func<A, B, C> selector)
        {
            return env =>
            {
                var first = reader(env);
                var second = first.Bind(a => binder(a)(env));
                return first.Bind(a => second.Map(b => selector(a, b)));
            };
        }
        
        // TODO: Generalize this or find this function in API !!
        public static Monad.Reader<E, Task<B>> Select<E, A, B>(this Monad.Reader<E, Task<A>> reader, Func<A, B> selector)
        {
            return env => reader(env).Map(r => selector(r));
        }
    }
}
