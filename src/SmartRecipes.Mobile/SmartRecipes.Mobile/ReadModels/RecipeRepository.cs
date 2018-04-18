using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using LanguageExt;

namespace SmartRecipes.Mobile
{
    public class RecipeRepository : Repository
    {
        public RecipeRepository(ApiClient apiClient, Database database) : base(apiClient, database)
        {
        }

        public async Task<IEnumerable<Recipe>> GetAllAsync()
        {
            return await RetrievalAction(
                client => client.GetMyRecipes(),
                db => db.Recipes.ToEnumerableAsync(),
                response => response.Recipes.Select(r => ToRecipe(r)),
                recipes => recipes.Concat(recipes.Select(r => (object)r.Ingredients))
            );
        }

        private Recipe ToRecipe(MyRecipesResponse.Recipe r)
        {
            var ingredients = r.Ingredients.Select(i => Ingredient.CreateForRecipe(i.Id, r.Id, i.FoodstuffId, i.Amount));
            return Recipe.Create(r.Id, r.OwnerId, r.Name, r.ImageUrl, r.PersonCount, r.Text, ingredients);
        }
    }
}
