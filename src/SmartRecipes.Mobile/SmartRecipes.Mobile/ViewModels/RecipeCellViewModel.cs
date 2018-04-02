using System;

namespace SmartRecipes.Mobile
{
    public class RecipeCellViewModel
    {
        private RecipeCellViewModel(Recipe recipe, Action onPlus, Action onOther = null, string otherActionIcon = "detail")
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

        public static RecipeCellViewModel Create(Recipe recipe, Action onPlus, Action onOther = null, string otherActionIcon = "detail")
        {
            return new RecipeCellViewModel(recipe, onPlus, onOther, otherActionIcon);
        }
    }
}
