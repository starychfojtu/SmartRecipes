using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LanguageExt;
using SmartRecipes.Mobile.Services;

namespace SmartRecipes.Mobile
{
    public static class Repository
    {
        public static async Task<TModel> RetrievalAction<TModel, TResponse>(
            DataAccess dataAccess,
            Func<ApiClient, Task<Option<TResponse>>> apiCall,
            Func<Database, Task<TModel>> databaseQuery,
            Func<TResponse, TModel> responseMapper,
            Func<TModel, IEnumerable<object>> databaseMapper)
        {
            var response = await apiCall(dataAccess.Api);
            return await response.MatchAsync(
                async r =>
                {
                    var model = responseMapper(r);

                    foreach (var item in databaseMapper(model))
                    {
                        await dataAccess.Db.AddOrReplaceAsync(item);
                    }

                    return model;
                },
                async () =>
                {
                    return await databaseQuery(dataAccess.Db);
                }
            );
        }
    }
}
