using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SmartRecipes.Mobile.ReadModels;
using SmartRecipes.Mobile.Services;

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
            Recipes = recipes.Select(r => new RecipeCellViewModel(r, () => { }));
            RaisePropertyChanged(nameof(Recipes));
        }

        public override async Task InitializeAsync()
        {
            await UpdateRecipesAsync();
        }
    }
}
