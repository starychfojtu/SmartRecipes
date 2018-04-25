using System;
using System.Threading.Tasks;
using SmartRecipes.Mobile.Models;

namespace SmartRecipes.Mobile.ViewModels
{
    public class FoodstuffCellViewModel
    {
        public FoodstuffCellViewModel(IFoodstuff foodstuff, IAmount amount, Func<Task> onPlus, Func<Task> onMinus = null)
        {
            Foodstuff = foodstuff;
            Amount = amount;
            OnPlus = onPlus;
            OnMinus = onMinus;
        }

        public IFoodstuff Foodstuff { get; }

        public IAmount Amount { get; }

        public Func<Task> OnPlus { get; }

        public Func<Task> OnMinus { get; }
    }
}
