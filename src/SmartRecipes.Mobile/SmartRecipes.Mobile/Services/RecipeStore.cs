using System.Collections.Generic;

namespace SmartRecipes.Mobile
{
    public partial class Store
    {
        private IEnumerable<Recipe> myRecipes;

        public IEnumerable<Recipe> MyRecipes
        {
            get { return myRecipes ?? (myRecipes = GetMyRecipes()); }
        }

        public IEnumerable<Recipe> GetMyRecipes()
        {
            return new[]
            {
                Recipe.Create("Lasagna", null, null, 1, ShoppingListItems, "Cook me"),
                Recipe.Create("Lasagna 2", null, null, 2, ShoppingListItems, "Cook me twice")
            };
        }
    }
}
