using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LanguageExt;
using SmartRecipes.Mobile.ApiDto;
using SmartRecipes.Mobile.Models;
using SmartRecipes.Mobile.ReadModels.Dto;
using SmartRecipes.Mobile.Services;

namespace SmartRecipes.Mobile.ReadModels
{
    public static class RecipeRepository
    {
        public static async Task<IEnumerable<IRecipe>> GetRecipesAsync(ApiClient apiClient, Database database)
        {
            return await Repository.RetrievalAction(
                apiClient,
                database,
                client => client.GetMyRecipes(),
                db => db.Recipes.ToEnumerableAsync(),
                response => response.Recipes.Select(r => ToRecipe(r)),
                recipes => recipes
            );
        }

        private static async Task<IEnumerable<RecipeDetail>> GetRecipeDetails(Database database)
        {
            // TODO: write double join query or simplify this by abstraction, this code is horrible, split it
            var recipes = await database.Recipes.ToEnumerableAsync();
            var recipeIds = recipes.Select(r => r.Id);
            var foodstuffAmounts = await database.IngredientAmounts.Where(a => recipeIds.Contains(a.RecipeId)).ToEnumerableAsync();
            var foodsstuffIds = foodstuffAmounts.Select(a => a.FoodstuffId);
            var foodstuffs = await database.Foodstuffs.Where(f => foodsstuffIds.Contains(f.Id)).ToEnumerableAsync();
            var recipesWithAmounts = recipes.GroupJoin(foodstuffAmounts, r => r.Id, a => a.RecipeId, (r, a) => new { Recipe = r, Amounts = a });
            return recipesWithAmounts.SelectMany(
                recipeWithAmounts => recipeWithAmounts.Amounts.GroupJoin(
                    foodstuffs,
                    a => a.FoodstuffId,
                    f => f.Id,
                    (a, fs) => new RecipeDetail(recipeWithAmounts.Recipe, fs.Select(f => new Ingredient(f, a)).ToSomeEnumerable())
                )
            );
        }

        private static Recipe ToRecipe(MyRecipesResponse.Recipe r)
        {
            return Recipe.Create(r.Id, r.OwnerId, r.Name, r.ImageUrl, r.PersonCount, r.Text);
        }
    }
}
