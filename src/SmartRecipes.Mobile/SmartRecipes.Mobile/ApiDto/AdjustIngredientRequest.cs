using System;

namespace SmartRecipes.Mobile.ApiDto
{
    public enum IngredientAction
    {
        DecreaseAmount,
        IncreaseAmount
    }

    public class AdjustIngredientRequest
    {
        public AdjustIngredientRequest(Guid foodstuffId, IngredientAction action)
        {
            FoodstuffId = foodstuffId;
            Action = action;
        }

        public Guid FoodstuffId { get; }

        public IngredientAction Action { get; set; }
    }
}
