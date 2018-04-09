using System;

namespace SmartRecipes.Mobile
{
    public class FoodstuffCellViewModel
    {
        public FoodstuffCellViewModel(Foodstuff foodstuff, Amount amount, Action onPlus, Action onMinus = null)
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
    }
}
