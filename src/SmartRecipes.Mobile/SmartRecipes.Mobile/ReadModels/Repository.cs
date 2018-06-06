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
            ApiClient apiClient,
            Database database,
            Func<ApiClient, Task<Option<TResponse>>> apiCall,
            Func<Database, Task<TModel>> databaseQuery,
            Func<TResponse, TModel> responseMapper,
            Func<TModel, IEnumerable<object>> databaseMapper)
        {
            var response = await apiCall(apiClient);
            return await response.MatchAsync(
                async r =>
                {
                    var model = responseMapper(r);

                    foreach (var item in databaseMapper(model))
                    {
                        await database.AddOrReplaceAsync(item);
                    }

                    return model;
                },
                async () =>
                {
                    return await databaseQuery(database);
                }
            );
        }
    }
}
