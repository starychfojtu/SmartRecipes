using System;
using System.Threading.Tasks;
using LanguageExt;
using SmartRecipes.Mobile.Models;

namespace SmartRecipes.Mobile.ViewModels
{
    public class FoodstuffAmountCellViewModel
    {
        public FoodstuffAmountCellViewModel(Some<IFoodstuff> foodstuff, Some<IAmount> amount, Func<Task> onPlus, Func<Task> onMinus = null)
        {
            Foodstuff = foodstuff.Value;
            Amount = amount.Value;
            OnPlus = onPlus;
            OnMinus = onMinus;
        }

        public IFoodstuff Foodstuff { get; }

        public IAmount Amount { get; }

        public Func<Task> OnPlus { get; }

        public Func<Task> OnMinus { get; }
    }
}
