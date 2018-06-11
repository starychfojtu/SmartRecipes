using System.Threading.Tasks;
using SmartRecipes.Mobile.Models;
using SmartRecipes.Mobile.Services;
using System.Collections.Generic;

namespace SmartRecipes.Mobile.WriteModels
{
    public static class MyRecipesHandler
    {
        public static async Task Add(
            ApiClient apiClient,
            Database database,
            IRecipe recipe,
            IEnumerable<IFoodstuffAmount> ingredients)
        {
            // TODO: add to API or job
            await database.AddAsync(recipe.ToEnumerable());
            await database.AddAsync(ingredients);
        }

        public static async Task Update(ApiClient apiClient, Database database, IRecipe recipe, IEnumerable<IIngredientAmount> ingredients)
        {
            await database.UpdateAsync(recipe.ToEnumerable());
            await DeleteIngredients(database, recipe);
            await database.AddAsync(ingredients);
        }

        private static async Task DeleteIngredients(Database database, IRecipe recipe)
        {
            var ingredientAmounts = database.GetTableMapping<IngredientAmount>();
            var recipeId = ingredientAmounts.FindColumnWithPropertyName(nameof(IngredientAmount.RecipeId)).Name;
            var deleteCommand = $@"DELETE FROM {ingredientAmounts.TableName} WHERE {recipeId} = ?";

            await database.Execute<int>(deleteCommand, recipe.Id);
        }
    }
}
