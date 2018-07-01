using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LanguageExt;
using SmartRecipes.Mobile.Extensions;
using SmartRecipes.Mobile.Infrastructure;
using SmartRecipes.Mobile.ReadModels;
using SmartRecipes.Mobile.WriteModels;
using SmartRecipes.Mobile.Models;
using static LanguageExt.Prelude;

namespace SmartRecipes.Mobile.ViewModels
{
    public class MyRecipesViewModel : ViewModel
    {
        private readonly Enviroment enviroment;

        public MyRecipesViewModel(Enviroment enviroment)
        {
            this.enviroment = enviroment;
        }

        public IEnumerable<RecipeCellViewModel> Recipes { get; private set; }

        public override async Task InitializeAsync()
        {
            await UpdateRecipesAsync();
        }

        public Task AddRecipe()
        {
            return Navigation.CreateRecipe();
        }

        public async Task UpdateRecipesAsync()
        {
            var recipeDetails = await RecipeRepository.GetMyRecipeDetails()(enviroment);
            Recipes = recipeDetails.Select(detail => new RecipeCellViewModel(
                detail,
                None,
                new UserAction<IRecipe>(r => AddToShoppingList(r), Icon.CartAdd(), 1),
                new UserAction<IRecipe>(r => EditRecipe(r), Icon.Edit(), 2),
                new UserAction<IRecipe>(r => DeleteRecipe(r), Icon.Delete(), 3)
            ));
            RaisePropertyChanged(nameof(Recipes));
        }

        public async Task<Option<UserMessage>> EditRecipe(IRecipe recipe)
        {
            var detail = await RecipeRepository.GetDetail(recipe)(enviroment);
            await Navigation.EditRecipe(detail);
            return None;
        }

        public Task<Option<UserMessage>> DeleteRecipe(IRecipe recipe)
        {
            return MyRecipesHandler.Delete(enviroment, recipe)
                .Bind(_ => TryAsync(() => InitializeAsync().ContinueWith(r => Unit.Default)))
                .MapToUserMessage(_ => UserMessage.Deleted());
        }

        private Task<Option<UserMessage>> AddToShoppingList(IRecipe recipe)
        {
            return ShoppingListHandler
                .AddToShoppingList(enviroment, recipe, CurrentAccount, recipe.PersonCount)
                .MapToUserMessage(_ => UserMessage.Added());
        }
    }
}
