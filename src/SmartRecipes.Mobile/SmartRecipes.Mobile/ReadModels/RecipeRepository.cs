using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SmartRecipes.Mobile.ApiDto;
using SmartRecipes.Mobile.Models;
using SmartRecipes.Mobile.ReadModels.Dto;
using SmartRecipes.Mobile.Services;
using System.Collections.Immutable;
using System;
using LanguageExt;
using LanguageExt.SomeHelp;

namespace SmartRecipes.Mobile.ReadModels
{
    public static class RecipeRepository
    {
        public static async Task<IEnumerable<IRecipe>> GetRecipesAsync(DataAccess dataAccess)
        {
            return await Repository.RetrievalAction(
                dataAccess,
                client => client.GetMyRecipes(),
                db => db.Recipes.ToEnumerableAsync(),
                response => response.Recipes.Select(r => ToRecipe(r)),
                recipes => recipes
            );
        }

        public static Reader<DataAccess, Task<RecipeDetail>> GetDetail(Guid recipeId)
        {
            return GetRecipe(recipeId).Bind(GetDetail);
        }

        public static Reader<DataAccess, Task<RecipeDetail>> GetDetail(IRecipe recipe)
        {
            return GetIngredients(recipe.ToSome()).Select(t => t.Map(i => new RecipeDetail(recipe.ToSome(), i.ToSomeEnumerable())));
        }

        public static Reader<DataAccess, Task<IRecipe>> GetRecipe(Guid recipeId)
        {
            return da => (da.Db.Recipes.Where(r => r.Id == recipeId).FirstAsync().Map(r => r as IRecipe), false);
        }

        public static Reader<DataAccess, Task<IEnumerable<Ingredient>>> GetIngredients(Some<IRecipe> recipe)
        {
            return da =>
                (da.Db.IngredientAmounts.Where(i => i.RecipeId == recipe.Value.Id).ToEnumerableAsync()
                    .SelectMany(
                      amounts => da.Db.Foodstuffs.Where(f => amounts.Select(i => i.FoodstuffId).Contains(f.Id)).ToEnumerableAsync(),
                      (amounts, foodstuffs) => amounts.Join(foodstuffs, i => i.FoodstuffId, f => f.Id, (i, f) => new Ingredient(f, i))
                ), false);
        }

        private static Recipe ToRecipe(MyRecipesResponse.Recipe r)
        {
            return Recipe.Create(r.Id, r.OwnerId, r.Name, r.ImageUrl, r.PersonCount, r.Text);
        }
    }
}
