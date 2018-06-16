using System;
using LanguageExt;
using SmartRecipes.Mobile.Models;
namespace SmartRecipes.Mobile.ReadModels.Dto
{
    public class ShoppingListRecipeItem
    {
        public ShoppingListRecipeItem(Some<RecipeDetail> detail, Some<IRecipeInShoppingList> recipeInShoppingList)
        {
            Detail = detail;
            RecipeInShoppingList = recipeInShoppingList.Value;
        }

        public IRecipeInShoppingList RecipeInShoppingList { get; }

        public RecipeDetail Detail { get; }
    }
}
