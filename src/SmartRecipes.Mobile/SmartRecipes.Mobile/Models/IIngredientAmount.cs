using System;

namespace SmartRecipes.Mobile.Models
{
    public interface IIngredientAmount : IFoodstuffAmount
    {
        Guid RecipeId { get; }

        IIngredientAmount WithAmount(IAmount amount);

        // IFoodstuffAmount WithRecipe(IRecipe recipe);
    }
}
