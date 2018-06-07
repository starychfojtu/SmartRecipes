using System;

namespace SmartRecipes.Mobile.Models
{
    public interface IShoppingListItemAmount : IFoodstuffAmount
    {
        Guid ShoppingListOwnerId { get; }

        IShoppingListItemAmount WithAmount(IAmount amount);
    }
}
