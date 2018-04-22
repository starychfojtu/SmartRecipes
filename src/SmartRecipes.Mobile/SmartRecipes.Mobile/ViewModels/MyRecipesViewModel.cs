using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SmartRecipes.Mobile.ReadModels;
using SmartRecipes.Mobile.Services;
using SmartRecipes.Mobile.WriteModels;

namespace SmartRecipes.Mobile.ViewModels
{
    public class MyRecipesViewModel : ViewModel
    {
        private readonly MyRecipesHandler commandHandler;

        private readonly RecipeRepository repository;

        public MyRecipesViewModel(MyRecipesHandler commandHandler, RecipeRepository repository)
        {
            this.repository = repository;
            this.commandHandler = commandHandler;
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
