using System.Threading.Tasks;
using SmartRecipes.Mobile.Models;
using SmartRecipes.Mobile.Services;
using System.Collections.Generic;

namespace SmartRecipes.Mobile.WriteModels
{
    public static class MyRecipesHandler
    {
        public static async Task Add(
            Database database,
            IRecipe recipe,
            IEnumerable<IFoodstuffAmount> ingredients)
        {
            // TODO: add to API or job
            await database.AddAsync(recipe.ToEnumerable());
            await database.AddAsync(ingredients);
        }
    }
}
