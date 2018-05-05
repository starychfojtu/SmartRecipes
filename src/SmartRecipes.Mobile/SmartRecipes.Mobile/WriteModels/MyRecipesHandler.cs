using System.Threading.Tasks;
using SmartRecipes.Mobile.Models;
using SmartRecipes.Mobile.Services;
using System.Collections;
using System.Collections.Generic;

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

        public async Task Add(IRecipe recipe, IEnumerable<IFoodstuffAmount> ingredients)
        {
            // TODO: add to API or job
            await database.AddAsync(recipe.ToEnumerable());
            await database.AddAsync(ingredients);
        }
    }
}
