using System;

namespace SmartRecipes.Mobile.Models
{
    public interface IFoodstuffAmount
    {
        Guid Id { get; }

        Guid FoodstuffId { get; }

        IAmount Amount { get; }
    }
}
