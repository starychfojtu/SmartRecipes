using System;
using SmartRecipes.Mobile.Models;

namespace SmartRecipes.Mobile.ViewModels
{
    public class RecipeCellViewModel
    {
        public RecipeCellViewModel(IRecipe recipe, Action onPlus, Action onOther = null, string otherActionIcon = "detail")
        {
            OtherActionIcon = otherActionIcon;
            Recipe = recipe;
            OnPlus = onPlus;
            OnOther = onOther;
        }

        public IRecipe Recipe { get; }

        public Action OnPlus { get; }

        public Action OnOther { get; }

        public string OtherActionIcon { get; }
    }
}
