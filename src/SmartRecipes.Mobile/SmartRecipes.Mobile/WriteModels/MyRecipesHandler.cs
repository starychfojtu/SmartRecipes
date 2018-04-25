using System.Threading.Tasks;
using SmartRecipes.Mobile.Models;
using SmartRecipes.Mobile.Services;

namespace SmartRecipes.Mobile.WriteModels
{
    public class MyRecipesHandler
    {
        private readonly ApiClient apiClient;

        private readonly Database database;

        public MyRecipesHandler(ApiClient apiClient, Database database)
        {
            this.database = database;
            this.apiClient = apiClient;
        }

        public async Task Add(IRecipe recipe)
        {
            await database.AddAsync(recipe.ToEnumerable());
        }
    }
}
