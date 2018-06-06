using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SmartRecipes.Mobile.ReadModels;
using SmartRecipes.Mobile.Services;

namespace SmartRecipes.Mobile.ViewModels
{
    public class MyRecipesViewModel : ViewModel
    {
        private readonly RecipeRepository repository;

        public MyRecipesViewModel(RecipeRepository repository)
        {
            this.repository = repository;
        }

        public IEnumerable<RecipeCellViewModel> Recipes { get; private set; }

        public async Task AddRecipe()
        {
            await Navigation.AddRecipe(this);
        }

        public async Task Refresh()
        {
            await InitializeAsync();
        }

        public async Task UpdateRecipesAsync()
        {
            var recipes = await repository.GetRecipesAsync();
            Recipes = recipes.Select(r => new RecipeCellViewModel(r, () => { }));
            RaisePropertyChanged(nameof(Recipes));
        }

        public override async Task InitializeAsync()
        {
            await UpdateRecipesAsync();
        }
    }
}
