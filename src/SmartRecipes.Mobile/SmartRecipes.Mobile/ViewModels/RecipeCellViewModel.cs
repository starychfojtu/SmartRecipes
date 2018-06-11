using System;
using SmartRecipes.Mobile.Models;
using SmartRecipes.Mobile.ReadModels.Dto;
using System.Threading.Tasks;
using SmartRecipes.Mobile.Services;

namespace SmartRecipes.Mobile.ViewModels
{
    public class RecipeCellViewModel
    {
        private readonly Func<IRecipe, Task<RecipeDetail>> getDetail;

        public RecipeCellViewModel(
            IRecipe recipe,
            Func<IRecipe, Task<RecipeDetail>> getDetail,
            Func<Task> onPlus,
            Action onOther = null,
            string otherActionIcon = "detail")
        {
            this.getDetail = getDetail;
            OtherActionIcon = otherActionIcon;
            Recipe = recipe;
            OnPlus = onPlus;
            OnOther = onOther;
        }

        public IRecipe Recipe { get; }

        public Func<Task> OnPlus { get; }

        public Action OnOther { get; }

        public string OtherActionIcon { get; }

        public async Task EditRecipe()
        {
            await Navigation.EditRecipe(await getDetail(Recipe));
        }
    }
}
