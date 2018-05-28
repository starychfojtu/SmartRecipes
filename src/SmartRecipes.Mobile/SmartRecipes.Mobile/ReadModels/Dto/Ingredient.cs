using LanguageExt;
using SmartRecipes.Mobile.Models;
using LanguageExt.SomeHelp;
using System;

namespace SmartRecipes.Mobile.ReadModels.Dto
{
    public class Ingredient
    {
        public Ingredient(Some<IFoodstuff> foodstuff, Some<IFoodstuffAmount> foodstuffAmount)
        {
            Foodstuff = foodstuff.Value;
            FoodstuffAmount = foodstuffAmount.Value;
        }

        public IFoodstuff Foodstuff { get; }

        public IFoodstuffAmount FoodstuffAmount { get; }

        public IAmount Amount
        {
            get { return FoodstuffAmount.Amount; }
        }

        public Ingredient WithAmount(Some<IAmount> amount)
        {
            return new Ingredient(Foodstuff.ToSome(), FoodstuffAmount.WithAmount(amount.Value).ToSome());
        }
    }
}
