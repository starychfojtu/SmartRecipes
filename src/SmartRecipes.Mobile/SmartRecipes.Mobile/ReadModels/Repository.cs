using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LanguageExt;

namespace SmartRecipes.Mobile
{
    public abstract class Repository
    {
        private readonly ApiClient apiClient;

        private readonly Database database;

        public Repository(ApiClient apiClient, Database database)
        {
            this.apiClient = apiClient;
            this.database = database;
        }

        protected async Task<TModel> RetrievalAction<TModel, TResponse>(
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
