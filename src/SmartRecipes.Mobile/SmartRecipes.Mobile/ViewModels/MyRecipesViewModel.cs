using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SmartRecipes.Mobile.Controllers;

namespace SmartRecipes.Mobile
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

        public async Task UpdateRecipesAsync()
        {
            var recipes = await repository.GetAllAsync();
            Recipes = recipes.Select(r => RecipeCellViewModel.Create(r, () => { }));
            RaisePropertyChanged(nameof(Recipes));
        }

        public override async Task InitializeAsync()
        {
            await UpdateRecipesAsync();
        }
    }
}
