using LanguageExt;
using SmartRecipes.Mobile.Models;
using LanguageExt.SomeHelp;

namespace SmartRecipes.Mobile.ReadModels.Dto
{
    public class Ingredient
    {
        public Ingredient(IFoodstuff foodstuff, IIngredientAmount ingredientAmount)
        {
            Foodstuff = foodstuff;
            IngredientAmount = ingredientAmount;
        }

        public IFoodstuff Foodstuff { get; }

        public IIngredientAmount IngredientAmount { get; }

        public IAmount Amount
        {
            get { return IngredientAmount.Amount; }
        }

        public Ingredient WithAmount(IAmount amount)
        {
            return new Ingredient(Foodstuff, IngredientAmount.WithAmount(amount));
        }
    }
}
