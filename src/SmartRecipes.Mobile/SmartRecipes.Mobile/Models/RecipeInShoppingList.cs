using System;
using LanguageExt;

namespace SmartRecipes.Mobile.Models
{
    public sealed class RecipeInShoppingList : Entity, IRecipeInShoppingList
    {
        private RecipeInShoppingList(Guid id, Guid recipeId, Guid shoppingListOwnerId, int personCount) : base(id)
        {
            RecipeId = recipeId;
            ShoppingListOwnerId = shoppingListOwnerId;
            PersonCount = personCount;
        }

        public RecipeInShoppingList() : base(Guid.Empty) { /* sqlite */ }

        public Guid RecipeId { get; set; }

        public Guid ShoppingListOwnerId { get; set; }

        public int PersonCount { get; set; }

        public static IRecipeInShoppingList Create(Guid id, Guid recipeId, Guid shoppingListOwnerId, int personCount)
        {
            return new RecipeInShoppingList(id, recipeId, shoppingListOwnerId, personCount);
        }

        public static IRecipeInShoppingList Create(Some<IRecipe> recipe, Some<IAccount> owner, int personCount)
        {
            return new RecipeInShoppingList(Guid.NewGuid(), recipe.Value.Id, owner.Value.Id, personCount);
        }
    }
}
