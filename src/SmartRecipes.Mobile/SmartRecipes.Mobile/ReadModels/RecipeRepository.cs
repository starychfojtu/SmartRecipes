using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SmartRecipes.Mobile.ApiDto;
using SmartRecipes.Mobile.Models;
using SmartRecipes.Mobile.ReadModels.Dto;
using SmartRecipes.Mobile.Services;
using System;
using SmartRecipes.Mobile.Extensions;

namespace SmartRecipes.Mobile.ReadModels
{
    public static class RecipeRepository
    {
        public static Monad.Reader<DataAccess, Task<IEnumerable<IRecipe>>> GetRecipes()
        {
            return Repository.RetrievalAction(
                client => client.GetMyRecipes(),
                env => env.Db.Recipes.ToEnumerableAsync<Recipe, IRecipe>(),
                response => response.Recipes.Select(r => ToRecipe(r)),
                recipes => recipes
            );
        }

        public static Monad.Reader<DataAccess, Task<RecipeDetail>> GetDetail(Guid recipeId)
        {
            return GetRecipe(recipeId).Bind(r => GetDetail(r));
        }

        public static Monad.Reader<DataAccess, Task<RecipeDetail>> GetDetail(IRecipe recipe)
        {
            return GetIngredients(recipe).Select(i => new RecipeDetail(recipe, i));
        }

        public static Monad.Reader<DataAccess, Task<IEnumerable<RecipeDetail>>> GetDetails(IEnumerable<IRecipe> recipes)
        {
            return env => Task.WhenAll(recipes.Select(r => GetDetail(r)(env))).Map(ds => ds as IEnumerable<RecipeDetail>);
        }

        public static Monad.Reader<DataAccess, Task<IRecipe>> GetRecipe(Guid recipeId)
        {
            return GetRecipes(recipeId.ToEnumerable()).Select(rs => rs.First()); 
        }

        public static Monad.Reader<DataAccess, Task<IEnumerable<IRecipe>>> GetRecipes(IEnumerable<Guid> ids)
        {
            return env => env.Db.Recipes.Where(r => ids.Contains(r.Id)).ToEnumerableAsync<Recipe, IRecipe>();
        }

        public static Monad.Reader<DataAccess, Task<IEnumerable<Ingredient>>> GetIngredients(IRecipe recipe)
        {
            return 
                from ias in GetIngredientAmounts(recipe)
                from fs in FoodstuffRepository.GetFoodstuffs(ias.Select(i => i.FoodstuffId))
                select ias.Join(fs, i => i.FoodstuffId, f => f.Id, (i, f) => new Ingredient(f, i));
        }

        public static Monad.Reader<DataAccess, Task<IEnumerable<IngredientAmount>>> GetIngredientAmounts(IRecipe recipe)
        {
            return env => env.Db.IngredientAmounts.Where(i => i.RecipeId == recipe.Id).ToEnumerableAsync();
        }

        private static Recipe ToRecipe(MyRecipesResponse.Recipe r)
        {
            return Recipe.Create(r.Id, r.OwnerId, r.Name, r.ImageUrl, r.PersonCount, r.Text);
        }
    }
}
