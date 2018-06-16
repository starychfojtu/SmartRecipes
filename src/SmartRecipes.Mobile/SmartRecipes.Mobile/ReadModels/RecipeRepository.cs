using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SmartRecipes.Mobile.ApiDto;
using SmartRecipes.Mobile.Models;
using SmartRecipes.Mobile.ReadModels.Dto;
using SmartRecipes.Mobile.Services;
using System;
using LanguageExt;
using LanguageExt.SomeHelp;
using Monad;

namespace SmartRecipes.Mobile.ReadModels
{
    public static class RecipeRepository
    {
        public static Monad.Reader<DataAccess, Task<IEnumerable<IRecipe>>> GetRecipes()
        {
            return Repository.RetrievalAction(
                client => client.GetMyRecipes(),
                da => da.Db.Recipes.ToEnumerableAsync<Recipe, IRecipe>(),
                response => response.Recipes.Select(r => ToRecipe(r)),
                recipes => recipes
            );
        }

        public static Monad.Reader<DataAccess, Task<RecipeDetail>> GetDetail(Guid recipeId)
        {
            return GetRecipe(recipeId).Bind(GetDetail);
        }

        public static Monad.Reader<DataAccess, Task<RecipeDetail>> GetDetail(IRecipe recipe)
        {
            return GetIngredients(recipe).Select(t => t.Map(i => new RecipeDetail(recipe, i.ToSomeEnumerable())));
        }

        public static Monad.Reader<DataAccess, Task<IRecipe>> GetRecipe(Guid recipeId)
        {
            return da => da.Db.Recipes.Where(r => r.Id == recipeId).FirstAsync().Map(r => r as IRecipe);
        }

        public static Monad.Reader<DataAccess, Task<IEnumerable<Ingredient>>> GetIngredients(IRecipe recipe)
        {
            return GetIngredientAmounts(recipe).SelectMany(
                amounts => FoodstuffRepository.GetByIds(amounts.Select(a => a.FoodstuffId)),
                (amounts, foodstuffs) => amounts.Join(foodstuffs, i => i.FoodstuffId, f => f.Id, (i, f) => new Ingredient(f, i))
            );
        }

        public static Monad.Reader<DataAccess, Task<IEnumerable<IngredientAmount>>> GetIngredientAmounts(IRecipe recipe)
        {
            return da => da.Db.IngredientAmounts.Where(i => i.RecipeId == recipe.Id).ToEnumerableAsync();
        }

        private static Recipe ToRecipe(MyRecipesResponse.Recipe r)
        {
            return Recipe.Create(r.Id, r.OwnerId, r.Name, r.ImageUrl, r.PersonCount, r.Text);
        }
    }
}
