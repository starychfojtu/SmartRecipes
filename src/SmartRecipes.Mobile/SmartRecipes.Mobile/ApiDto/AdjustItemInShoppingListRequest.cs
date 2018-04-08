using System;

namespace SmartRecipes.Mobile.ApiDto
{
    public enum AdjustShoppingListItemAction
    {
        DecreaseAmount,
        IncreaseAmount
    }

    public class AdjustItemInShoppingListRequest
    {
        public AdjustItemInShoppingListRequest(Guid foodstuffId, AdjustShoppingListItemAction action)
        {
            FoodstuffId = foodstuffId;
            Action = action;
        }

        public Guid FoodstuffId { get; }

        public AdjustShoppingListItemAction Action { get; set; }
    }
}
