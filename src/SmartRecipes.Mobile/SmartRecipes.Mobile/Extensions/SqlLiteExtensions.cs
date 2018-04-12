using System.Threading.Tasks;
using SQLite;
using System.Collections.Generic;

namespace SmartRecipes.Mobile
{
    public static class SqlLiteExtensions
    {
        public static Task<IEnumerable<T>> ToEnumerableAsync<T>(this AsyncTableQuery<T> tableQuery)
            where T : new()
        {
            return tableQuery.ToListAsync().ContinueWith(r => (IEnumerable<T>)r);
        }
    }
}
