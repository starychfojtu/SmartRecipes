using System.Collections.Generic;

namespace SmartRecipes.Mobile
{
    public static class CollectionExtensions
    {
        public static IList<T> Replpace<T>(this IList<T> list, T item, T replacement)
        {
            var index = list.IndexOf(item);
            list[index] = replacement;
            return list;
        }
    }
}
