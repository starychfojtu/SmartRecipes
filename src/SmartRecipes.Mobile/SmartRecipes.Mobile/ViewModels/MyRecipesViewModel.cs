using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LanguageExt;
using SmartRecipes.Mobile.Extensions;
using SmartRecipes.Mobile.Infrastructure;
using SmartRecipes.Mobile.ReadModels;
using SmartRecipes.Mobile.Services;
using SmartRecipes.Mobile.WriteModels;
using SmartRecipes.Mobile.Models;
using static LanguageExt.Prelude;

namespace SmartRecipes.Mobile.ViewModels
{
    public class MyRecipesViewModel : ViewModel
    {
        private readonly Enviroment _enviroment;

        public MyRecipesViewModel(Enviroment enviroment)
        {
            this._enviroment = enviroment;
        }

        public IEnumerable<RecipeCellViewModel> Recipes { get; private set; }

        public async Task AddRecipe()
        {
            await Navigation.CreateRecipe();
        }

        public async Task UpdateRecipesAsync()
        {
            var recipes = await RecipeRepository.GetRecipes()(_enviroment);
            Recipes = recipes.Select(recipe => new RecipeCellViewModel(
                recipe,
                new UserAction<IRecipe>(r => AddToShoppingList(r), Icon.Plus(), 1),
                new UserAction<IRecipe>(r => EditRecipe(r), Icon.Minus(), 2)
            ));
            RaisePropertyChanged(nameof(Recipes));
        }

        public override async Task InitializeAsync()
        {
            await UpdateRecipesAsync();
        }
        
        public async Task<Option<UserMessage>> EditRecipe(IRecipe recipe)
        {
            var detail = await RecipeRepository.GetDetail(recipe)(_enviroment);
            await Navigation.EditRecipe(detail);
            return None;
        }

        private Task<Option<UserMessage>> AddToShoppingList(IRecipe recipe)
        {
            return ShoppingListHandler
                .AddToShoppingList(_enviroment, recipe, CurrentAccount, recipe.PersonCount)
                .ToUserMessage(_ => UserMessage.Added());
        }
    }
}
