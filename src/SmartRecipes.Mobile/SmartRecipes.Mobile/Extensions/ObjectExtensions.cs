using System;
using System.Collections.Generic;

namespace SmartRecipes.Mobile
{
    public static class ObjectExtensions
    {
        public static IEnumerable<T> ToEnumerable<T>(this T obj)
        {
            yield return obj;
        }
    }
}
