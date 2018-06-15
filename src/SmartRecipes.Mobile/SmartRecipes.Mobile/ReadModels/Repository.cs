using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LanguageExt;
using SmartRecipes.Mobile.Services;

namespace SmartRecipes.Mobile
{
    public static class Repository
    {
        public static Monad.Reader<DataAccess, Task<TModel>> RetrievalAction<TModel, TResponse>(
            Func<ApiClient, Task<Option<TResponse>>> apiCall,
            Monad.Reader<DataAccess, Task<TModel>> databaseQuery,
            Func<TResponse, TModel> responseMapper,
            Func<TModel, IEnumerable<object>> databaseMapper)
        {
            return da => apiCall(da.Api).Bind(ro => ro.Match(
                r =>
                {
                    var model = responseMapper(r);
                    var newItems = databaseMapper(model);
                    var updateTask = newItems.Fold(Task.FromResult(0), (t, i) => t.Bind(_ => da.Db.AddOrReplaceAsync(i)));
                    return updateTask.Map(_ => model);
                },
                () => databaseQuery(da)
            ));
        }
    }
}
