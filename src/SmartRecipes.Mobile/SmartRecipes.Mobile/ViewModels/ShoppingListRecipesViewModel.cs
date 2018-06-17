using SmartRecipes.Mobile.Services;
using System.Collections.Generic;
using System.Collections.Immutable;
using SmartRecipes.Mobile.ReadModels.Dto;
using System.Linq;
using SmartRecipes.Mobile.Models;
using System.Threading.Tasks;
using SmartRecipes.Mobile.WriteModels;
using SmartRecipes.Mobile.ReadModels;

namespace SmartRecipes.Mobile.ViewModels
{
    public class ShoppingListRecipesViewModel : ViewModel
    {
        private readonly DataAccess dataAccess;

        private IImmutableList<ShoppingListRecipeItem> recipeItems;

        public ShoppingListRecipesViewModel(DataAccess dataAccess)
        {
            this.dataAccess = dataAccess;
            recipeItems = ImmutableList.Create<ShoppingListRecipeItem>();
        }

        public IEnumerable<RecipeCellViewModel> Recipes
        {
            get { return recipeItems.Select(i => ToViewModel(i)); }
        }

        public override async Task InitializeAsync()
        {
            UpdateRecipeItems(await ShoppingListRepository.GetRecipeItems(CurrentAccount)(dataAccess));
        }

        // TODO: refactor this
        private async Task Cook(IRecipe recipe)
        {
            var item = recipeItems.First(r => r.Detail.Recipe.Equals(recipe));
            await ShoppingListHandler.Cook(dataAccess, item);
            UpdateRecipeItems(recipeItems.Remove(item));
        }

        private async Task Delete(IRecipe recipe)
        {
            var item = recipeItems.First(r => r.Detail.Recipe.Equals(recipe));
            await ShoppingListHandler.RemoveFromShoppingList(dataAccess, item.RecipeInShoppingList);
            UpdateRecipeItems(recipeItems.Remove(item));
        }

        private void UpdateRecipeItems(IEnumerable<ShoppingListRecipeItem> items)
        {
            recipeItems = items.ToImmutableList();
            RaisePropertyChanged(nameof(Recipes));
        }
        
        private RecipeCellViewModel ToViewModel(ShoppingListRecipeItem item)
        {
            return new RecipeCellViewModel(
                item.Detail.Recipe,
                r => Task.FromResult(item.Detail),
                new UserAction<IRecipe>(r => Cook(r), Icon.Done(), 1),
                new UserAction<IRecipe>(r => Delete(r), Icon.Delete(), 2)
            );
        }
    }
}
