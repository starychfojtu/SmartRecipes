using SmartRecipes.Mobile.Models;
namespace SmartRecipes.Mobile.ReadModels.Dto
{
    public class ShoppingListRecipeItem
    {
        public ShoppingListRecipeItem(RecipeDetail detail, IRecipeInShoppingList recipeInShoppingList)
        {
            Detail = detail;
            RecipeInShoppingList = recipeInShoppingList;
        }

        public IRecipeInShoppingList RecipeInShoppingList { get; }

        public RecipeDetail Detail { get; }
    }
}
