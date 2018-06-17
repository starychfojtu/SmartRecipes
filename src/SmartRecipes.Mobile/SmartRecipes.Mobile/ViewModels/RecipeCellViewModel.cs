using System;
using System.Collections.Generic;
using SmartRecipes.Mobile.Models;
using SmartRecipes.Mobile.ReadModels.Dto;
using System.Threading.Tasks;
using SmartRecipes.Mobile.Infrastructure;
using SmartRecipes.Mobile.Services;

namespace SmartRecipes.Mobile.ViewModels
{
    public class RecipeCellViewModel
    {
        public RecipeCellViewModel(
            IRecipe recipe,
            params UserAction<IRecipe>[] actions)
        {
            Recipe = recipe;
            Actions = actions;
        }

        public IRecipe Recipe { get; }
        
        public IEnumerable<UserAction<IRecipe>> Actions { get; }
    }
}
