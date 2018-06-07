using LanguageExt;
using SmartRecipes.Mobile.Models;
using LanguageExt.SomeHelp;

namespace SmartRecipes.Mobile.ReadModels.Dto
{
    public class Ingredient
    {
        public Ingredient(Some<IFoodstuff> foodstuff, Some<IIngredientAmount> ingredientAmount)
        {
            Foodstuff = foodstuff.Value;
            IngredientAmount = ingredientAmount.Value;
        }

        public IFoodstuff Foodstuff { get; }

        public IIngredientAmount IngredientAmount { get; }

        public IAmount Amount
        {
            get { return IngredientAmount.Amount; }
        }

        public Ingredient WithAmount(Some<IAmount> amount)
        {
            return new Ingredient(Foodstuff.ToSome(), IngredientAmount.WithAmount(amount.Value).ToSome());
        }
    }
}
