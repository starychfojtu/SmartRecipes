using System;
using System.Threading.Tasks;
using LanguageExt;
using SmartRecipes.Mobile.ReadModels.Dto;

namespace SmartRecipes.Mobile.ViewModels
{
    public class IngredientCellViewModel
    {
        public IngredientCellViewModel(Some<Ingredient> ingredient, Func<Task> onPlus, Func<Task> onMinus = null)
        {
            Ingredient = ingredient;
            OnPlus = onPlus;
            OnMinus = onMinus;
        }

        public Ingredient Ingredient { get; }

        public Func<Task> OnPlus { get; }

        public Func<Task> OnMinus { get; }
    }
}
