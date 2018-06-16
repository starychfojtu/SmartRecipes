using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SmartRecipes.Mobile.ReadModels;
using SmartRecipes.Mobile.Services;
using SmartRecipes.Mobile.WriteModels;
using LanguageExt.SomeHelp;

namespace SmartRecipes.Mobile.ViewModels
{
    public class MyRecipesViewModel : ViewModel
    {
        private readonly DataAccess dataAccess;

        public MyRecipesViewModel(DataAccess dataAccess)
        {
            this.dataAccess = dataAccess;
        }

        public IEnumerable<RecipeCellViewModel> Recipes { get; private set; }

        public async Task AddRecipe()
        {
            await Navigation.CreateRecipe();
        }

        public async Task Refresh()
        {
            await InitializeAsync();
        }

        public async Task UpdateRecipesAsync()
        {
            var recipes = await RecipeRepository.GetRecipes()(dataAccess);
            Recipes = recipes.Select(r => new RecipeCellViewModel(
                r,
                async recipe => await RecipeRepository.GetDetail(recipe)(dataAccess),
                async () => await ShoppingListHandler.AddToShoppingList(dataAccess, r, CurrentAccount, r.PersonCount) // TODO: add modal to choose count from
            ));
            RaisePropertyChanged(nameof(Recipes));
        }

        public override async Task InitializeAsync()
        {
            await UpdateRecipesAsync();
        }
    }
}
