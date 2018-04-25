using LanguageExt;
using SmartRecipes.Mobile.Models;
using System.Collections.Generic;
using System.Linq;

namespace SmartRecipes.Mobile.ReadModels.Dto
{
    public class RecipeDetail
    {
        public RecipeDetail(Some<IRecipe> recipe, IEnumerable<Some<Ingredient>> ingredients)
        {
            Recipe = recipe.Value;
            Ingredients = ingredients.Select(i => i.Value);
        }

        public IRecipe Recipe { get; }

        public IEnumerable<Ingredient> Ingredients { get; }
    }
}
