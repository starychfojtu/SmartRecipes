using LanguageExt;
using System.Collections.Generic;
using System.Linq;
namespace SmartRecipes.Mobile
{
    public class RecipeDetail
    {
        public RecipeDetail(Some<Recipe> recipe, IEnumerable<Some<Ingredient>> ingredients)
        {
            Recipe = recipe;
            Ingredients = ingredients.Select(i => i.Value);
        }

        public Recipe Recipe { get; }

        public IEnumerable<Ingredient> Ingredients { get; }
    }
}
