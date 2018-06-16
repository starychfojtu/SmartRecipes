using System;
using SmartRecipes.Mobile.Services;
using System.Collections.Generic;
using System.Collections.Immutable;
using SmartRecipes.Mobile.ReadModels.Dto;
using System.Linq;
using SmartRecipes.Mobile.Models;
using System.Threading.Tasks;
using SmartRecipes.Mobile.WriteModels;
using System.Collections;
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
            get { return recipeItems.Select(i => new RecipeCellViewModel(i.Detail.Recipe, r => Task.FromResult(i.Detail), null, null)); }
        }

        public override async Task InitializeAsync()
        {
            UpdateRecipeItems(await ShoppingListRepository.GetRecipeItems(CurrentAccount)(dataAccess));
        }

        private async Task Cook(ShoppingListRecipeItem item)
        {
            await ShoppingListHandler.Cook(dataAccess, item);
            UpdateRecipeItems(recipeItems.Remove(item));
        }

        private async Task Delete(ShoppingListRecipeItem item)
        {
            await ShoppingListHandler.RemoveFromShoppingList(dataAccess, item.RecipeInShoppingList);
            UpdateRecipeItems(recipeItems.Remove(item));
        }

        private void UpdateRecipeItems(IEnumerable<ShoppingListRecipeItem> items)
        {
            recipeItems = items.ToImmutableList();
            RaisePropertyChanged(nameof(Recipes));
        }
    }
}
