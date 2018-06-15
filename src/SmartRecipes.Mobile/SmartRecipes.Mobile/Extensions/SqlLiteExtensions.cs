using System.Threading.Tasks;
using SQLite;
using System.Collections.Generic;
using LanguageExt;
using static LanguageExt.Prelude;
using System;
using System.Linq;

namespace SmartRecipes.Mobile
{
    public static class SqlLiteExtensions
    {
        public static Task<IEnumerable<T>> ToEnumerableAsync<T>(this AsyncTableQuery<T> tableQuery)
            where T : new()
        {
            return tableQuery.ToListAsync().Map(t => (IEnumerable<T>)t);
        }

        public static Task<IEnumerable<U>> ToEnumerableAsync<T, U>(this AsyncTableQuery<T> tableQuery)
            where T : class, new()
            where U : class
        {
            return tableQuery.ToEnumerableAsync().Map(t => t.Select(i => i as U));
        }

        public static Task<Option<T>> FirstOptionAsync<T>(this AsyncTableQuery<T> tableQuery)
            where T : new()
        {
            return tableQuery.FirstOrDefaultAsync().Map(t => Optional(t));
        }
    }
}
