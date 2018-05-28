using System;
using SmartRecipes.Mobile.Models;

namespace SmartRecipes.Mobile.ApiDto
{
    public class ChangeFoodstuffAmountRequest
    {
        public ChangeFoodstuffAmountRequest(Guid foodstuffId, IAmount amount)
        {
            FoodstuffId = foodstuffId;
            Amount = amount;
        }

        public Guid FoodstuffId { get; }

        public IAmount Amount { get; set; }
    }
}
