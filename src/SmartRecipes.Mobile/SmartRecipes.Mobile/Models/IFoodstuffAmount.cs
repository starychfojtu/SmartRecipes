using System;
using SmartRecipes.Mobile.Models;

namespace SmartRecipes.Mobile.Models
{
    public interface IFoodstuffAmount
    {
        Guid Id { get; set; }

        Guid FoodstuffId { get; set; }

        Guid? RecipeId { get; set; }

        Guid? ShoppingListOwnerId { get; set; }

        IAmount Amount { get; }

        IFoodstuffAmount WithAmount(IAmount amount);
    }
}
