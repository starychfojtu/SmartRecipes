using System;
using SmartRecipes.Mobile.Services;
using System.Collections.Generic;
using System.Collections.Immutable;
using SmartRecipes.Mobile.ReadModels.Dto;
using System.Linq;
using SmartRecipes.Mobile.Models;
using System.Threading.Tasks;
using LanguageExt;
using SmartRecipes.Mobile.Extensions;
using SmartRecipes.Mobile.Infrastructure;
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
        
        private Task<Option<UserMessage>> RecipeDeleteAction(IRecipe recipe, Func<DataAccess, ShoppingListRecipeItem, TryAsync<Unit>> action)
        {
            var item = recipeItems.First(r => r.Detail.Recipe.Equals(recipe));
            return action(dataAccess, item).ToUserMessage(_ =>
            {
                UpdateRecipeItems(recipeItems.Remove(item));
                return UserMessage.Deleted();
            });
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
                new UserAction<IRecipe>(r => RecipeDeleteAction(r, (da, i) => ShoppingListHandler.Cook(da, i)), Icon.Done(), 1),
                new UserAction<IRecipe>(r => RecipeDeleteAction(r, (da, i) => ShoppingListHandler.RemoveFromShoppingList(da, i.RecipeInShoppingList)), Icon.Delete(), 2)
            );
        }
    }
}
