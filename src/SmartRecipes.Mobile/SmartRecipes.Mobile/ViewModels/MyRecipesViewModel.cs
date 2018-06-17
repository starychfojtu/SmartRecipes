using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SmartRecipes.Mobile.ReadModels;
using SmartRecipes.Mobile.Services;
using SmartRecipes.Mobile.WriteModels;
using SmartRecipes.Mobile.Models;

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

        public async Task UpdateRecipesAsync()
        {
            var recipes = await RecipeRepository.GetRecipes()(dataAccess);
            Recipes = recipes.Select(recipe => new RecipeCellViewModel(
                recipe,
                r => RecipeRepository.GetDetail(r)(dataAccess),
                new UserAction<IRecipe>(
                    r => ShoppingListHandler.AddToShoppingList(dataAccess, r, CurrentAccount, r.PersonCount),
                    Icon.Plus(), 
                    1
                )
            ));
            RaisePropertyChanged(nameof(Recipes));
        }

        public override async Task InitializeAsync()
        {
            await UpdateRecipesAsync();
        }
    }
}
