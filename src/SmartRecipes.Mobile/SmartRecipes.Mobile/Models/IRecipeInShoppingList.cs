using System;

namespace SmartRecipes.Mobile.Models
{
    public interface IRecipeInShoppingList
    {
        Guid Id { get; }

        Guid RecipeId { get; }

        Guid ShoppingListOwnerId { get; }

        int PersonCount { get; }
    }
}
