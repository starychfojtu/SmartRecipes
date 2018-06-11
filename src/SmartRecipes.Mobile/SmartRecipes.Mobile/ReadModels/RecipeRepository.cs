using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LanguageExt;
using SmartRecipes.Mobile.ApiDto;
using SmartRecipes.Mobile.Models;
using SmartRecipes.Mobile.ReadModels.Dto;
using SmartRecipes.Mobile.Services;
using LanguageExt.SomeHelp;
using System.Collections.Immutable;
using System;

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

        public static async Task<RecipeDetail> GetDetail(ApiClient apiClient, Database database, Some<IRecipe> recipe)
        {
            var ingredients = await GetIngredients(apiClient, database, recipe);
            return new RecipeDetail(recipe.ToSome(), ingredients.ToSomeEnumerable());
        }

        public static async Task<RecipeDetail> GetDetail(ApiClient apiClient, Database database, Guid recipeId)
        {
            var recipe = await GetRecipe(apiClient, database, recipeId);
            return await GetDetail(apiClient, database, recipe.ToSome());
        }

        public static async Task<IRecipe> GetRecipe(ApiClient apiClient, Database database, Guid recipeId)
        {
            return await database.Recipes.Where(r => r.Id == recipeId).FirstAsync();
        }

        public static async Task<IEnumerable<Ingredient>> GetIngredients(ApiClient apiClient, Database database, Some<IRecipe> recipe)
        {
            var recipeId = recipe.Value.Id;
            var ingredientAmounts = await database.IngredientAmounts.Where(i => i.RecipeId == recipeId).ToEnumerableAsync();

            var foodstuffIds = ingredientAmounts.Select(i => i.FoodstuffId).ToImmutableHashSet();
            var foodstuffs = await database.Foodstuffs.Where(f => foodstuffIds.Contains(f.Id)).ToEnumerableAsync();

            return ingredientAmounts.Join(foodstuffs, i => i.FoodstuffId, f => f.Id, (i, f) => new Ingredient(f, i));
        }

        private static Recipe ToRecipe(MyRecipesResponse.Recipe r)
        {
            return Recipe.Create(r.Id, r.OwnerId, r.Name, r.ImageUrl, r.PersonCount, r.Text);
        }
    }
}
