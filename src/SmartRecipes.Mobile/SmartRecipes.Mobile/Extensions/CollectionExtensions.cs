using System;
using System.Collections.Generic;
using System.Linq;
using LanguageExt;
using LanguageExt.SomeHelp;

namespace SmartRecipes.Mobile
{
    public static class CollectionExtensions
    {
        public static IList<T> Replace<T>(this IList<T> list, T item, T replacement)
        {
            return list.Tee(l => l[l.IndexOf(item)] = replacement);
        }

        public static ICollection<T> Without<T>(this ICollection<T> list, T item)
        {
            return list.Tee(l => l.Remove(item));
        }

        public static IEnumerable<Some<T>> ToSomeEnumerable<T>(this IEnumerable<T> enumerable)
        {
            return enumerable.Select(i => i.ToSome());
        }

        public static IList<T> AddRange<T>(this IList<T> list, IEnumerable<T> items)
        {
            if (list == null) throw new ArgumentNullException(nameof(list));
            if (items == null) throw new ArgumentNullException(nameof(items));

            if (list is List<T>)
            {
                ((List<T>)list).AddRange(items);
            }
            else
            {
                foreach (var item in items)
                {
                    list.Add(item);
                }
            }
            return list;
        }
    }
}
