using System;
using LanguageExt;

namespace SmartRecipes.Mobile
{
    public class FoodstuffCellViewModel
    {
        private FoodstuffCellViewModel(Foodstuff foodstuff, Amount amount, Action onPlus, Action onMinus = null)
        {
            Foodstuff = foodstuff;
            Amount = amount;
            OnPlus = onPlus;
            OnMinus = onMinus;
        }

        public Foodstuff Foodstuff { get; }

        public Amount Amount { get; }

        public Action OnPlus { get; }

        public Action OnMinus { get; }

        // Combinators

        public static FoodstuffCellViewModel Create(Foodstuff foodstuff, Amount amount, Action onPlus, Action onMinus = null)
        {
            return new FoodstuffCellViewModel(foodstuff, amount, onPlus, onMinus);
        }
    }
}
