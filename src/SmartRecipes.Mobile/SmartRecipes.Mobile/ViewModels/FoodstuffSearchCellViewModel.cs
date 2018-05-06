using System;
using SmartRecipes.Mobile.Models;

namespace SmartRecipes.Mobile.ViewModels
{
    public class FoodstuffSearchCellViewModel
    {
        public FoodstuffSearchCellViewModel(IFoodstuff foodstuff, Action onSelected)
        {
            Foodstuff = foodstuff;
            OnSelected = onSelected;
        }

        public IFoodstuff Foodstuff { get; }

        public Action OnSelected { get; }
    }
}
