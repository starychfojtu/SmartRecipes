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
        private readonly ApiClient apiClient;

        private readonly Database database;

        public MyRecipesViewModel(ApiClient apiClient, Database database)
        {
            this.apiClient = apiClient;
            this.database = database;
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
            var recipes = await RecipeRepository.GetRecipesAsync(apiClient, database);
            Recipes = recipes.Select(r => new RecipeCellViewModel(
                r,
                async recipe => await RecipeRepository.GetDetail(apiClient, database, recipe.ToSome()),
                async () => await ShoppingListHandler.AddToShoppingList(apiClient, database, r.ToSome(), CurrentAccount.ToSome(), r.PersonCount) // TODO: add modal to choose count from
            ));
            RaisePropertyChanged(nameof(Recipes));
        }

        public override async Task InitializeAsync()
        {
            await UpdateRecipesAsync();
        }
    }
}
