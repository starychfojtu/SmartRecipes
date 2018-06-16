using System;
using System.Collections.Generic;
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
            params UserAction<IRecipe>[] actions)
        {
            this.getDetail = getDetail;
            Recipe = recipe;
            Actions = actions;
        }

        public IRecipe Recipe { get; }
        
        public IEnumerable<UserAction<IRecipe>> Actions { get; }

        public async Task EditRecipe()
        {
            await Navigation.EditRecipe(await getDetail(Recipe));
        }
    }
}
