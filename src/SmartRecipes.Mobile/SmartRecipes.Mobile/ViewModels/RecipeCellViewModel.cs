using System.Collections.Generic;
using LanguageExt;
using SmartRecipes.Mobile.Models;
using SmartRecipes.Mobile.Infrastructure;
using SmartRecipes.Mobile.ReadModels.Dto;

namespace SmartRecipes.Mobile.ViewModels
{
    public class RecipeCellViewModel
    {
        public RecipeCellViewModel(RecipeDetail detail, Option<int> personCount, params UserAction<IRecipe>[] actions)
        {
            Detail = detail;
            Actions = actions;
            PersonCount = personCount.IfNone(detail.Recipe.PersonCount);
        }

        public RecipeDetail Detail { get; }
        
        public IEnumerable<UserAction<IRecipe>> Actions { get; }

        public int PersonCount { get; }
    }
}
