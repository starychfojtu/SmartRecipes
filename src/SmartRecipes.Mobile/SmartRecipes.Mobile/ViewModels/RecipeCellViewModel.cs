using System;
using SmartRecipes.Mobile.Models;

namespace SmartRecipes.Mobile.ViewModels
{
    public class RecipeCellViewModel
    {
        public RecipeCellViewModel(Recipe recipe, Action onPlus, Action onOther = null, string otherActionIcon = "detail")
        {
            OtherActionIcon = otherActionIcon;
            Recipe = recipe;
            OnPlus = onPlus;
            OnOther = onOther;
        }

        public Recipe Recipe { get; }

        public Action OnPlus { get; }

        public Action OnOther { get; }

        public string OtherActionIcon { get; }
    }
}
