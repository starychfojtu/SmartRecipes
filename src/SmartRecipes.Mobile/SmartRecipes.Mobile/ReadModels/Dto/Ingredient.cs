using LanguageExt;
using SmartRecipes.Mobile.Models;

namespace SmartRecipes.Mobile.ReadModels.Dto
{
    public class Ingredient
    {
        public Ingredient(Some<Foodstuff> foodstuff, Some<FoodstuffAmount> foodstuffAmount)
        {
            Foodstuff = foodstuff;
            FoodstuffAmount = foodstuffAmount;
        }

        public Foodstuff Foodstuff { get; }

        public FoodstuffAmount FoodstuffAmount { get; }

        public Amount Amount
        {
            get { return FoodstuffAmount.Amount; }
        }

        public Ingredient WithAmount(Some<Amount> amount)
        {
            return new Ingredient(Foodstuff, FoodstuffAmount.WithAmount(amount));
        }
    }
}
