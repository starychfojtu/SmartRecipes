using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Xamarin.Forms;
using SmartRecipes.Mobile.Controls;
using System.Threading.Tasks;

namespace SmartRecipes.Mobile
{
    public static class ObjectExtensions
    {
        public static IEnumerable<T> ToEnumerable<T>(this T obj)
        {
            yield return obj;
        }

        public static T Tee<T>(this T obj, Action<T> action)
        {
            action(obj);
            return obj;
        }

        public static async Task<T> TeeAsync<T>(this T obj, Func<T, Task> action)
        {
            await action(obj);
            return obj;
        }

        public static void Bind<TContext, TProperty>(this TContext context, BindableObject obj, BindableProperty property, Expression<Func<TContext, TProperty>> propertyAccessor)
        {
            obj.SetBinding(property, propertyAccessor.ToPropertyPathName());
        }

        public static void BindText<TContext, TProperty>(this TContext context, Entry entry, Expression<Func<TContext, TProperty>> propertyAccessor)
        {
            context.Bind(entry, Entry.TextProperty, propertyAccessor);
        }

        public static void BindErrors<TContext>(this TContext context, ValidatableEntry entry, Expression<Func<TContext, bool>> predicate)
        {
            context.Bind(entry, ValidatableEntry.IsValidProperty, predicate);
        }
    }
}
