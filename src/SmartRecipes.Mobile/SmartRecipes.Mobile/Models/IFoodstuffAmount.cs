using System;

namespace SmartRecipes.Mobile.Models
{
    public interface IFoodstuffAmount
    {
        Guid Id { get; }

        Guid FoodstuffId { get; }

        Guid? RecipeId { get; }

        Guid? ShoppingListOwnerId { get; }

        IAmount Amount { get; }

        IFoodstuffAmount WithAmount(IAmount amount);

        IFoodstuffAmount WithRecipe(IRecipe recipe);
    }
}
