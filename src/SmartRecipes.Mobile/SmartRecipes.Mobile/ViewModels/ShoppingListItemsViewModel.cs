using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SmartRecipes.Mobile.ReadModels;
using System;
using SmartRecipes.Mobile.WriteModels;
using SmartRecipes.Mobile.ReadModels.Dto;
using SmartRecipes.Mobile.Services;
using System.Collections.Immutable;
using static LanguageExt.Prelude;

namespace SmartRecipes.Mobile.ViewModels
{
    public class ShoppingListItemsViewModel : ViewModel
    {
        private readonly ApiClient apiClient;

        private readonly Database database;

        private IImmutableList<Ingredient> ingredients { get; set; }

        public ShoppingListItemsViewModel(ApiClient apiClient, Database database)
        {
            this.apiClient = apiClient;
            this.database = database;
            ingredients = ImmutableList.Create<Ingredient>();
        }

        public IEnumerable<IngredientCellViewModel> Ingredients
        {
            get { return ingredients.Select(i => ToViewModel(i)); }
        }

        public override async Task InitializeAsync()
        {
            await UpdateItemsAsync();
        }

        public async Task Refresh()
        {
            await InitializeAsync();
        }

        public async Task OpenAddIngredientDialog()
        {
            var selected = await Navigation.SelectFoodstuffDialog();
            var newIngredients = await ShoppingListHandler.Add(apiClient, database, CurrentAccount, selected);
            var allIngredients = ingredients.Concat(newIngredients).ToImmutableList();
            UpdateIngredients(allIngredients);
        }

        private async Task IngredientAction(Ingredient ingredient, Func<Ingredient, Ingredient> action)
        {
            var newIngredient = action(ingredient);
            var oldItem = ingredients.First(i => i.Foodstuff.Id == ingredient.Foodstuff.Id);
            var newIngredients = ingredients.Replace(oldItem, newIngredient);
            await ShoppingListHandler.Update(apiClient, database, newIngredient.FoodstuffAmount.ToEnumerable());
            UpdateIngredients(newIngredients);
        }

        private async Task UpdateItemsAsync()
        {
            UpdateIngredients((await ShoppingListRepository.GetItems(apiClient, database, CurrentAccount)).ToImmutableList());
        }

        private void UpdateIngredients(IImmutableList<Ingredient> newIngredients)
        {
            ingredients = newIngredients.OrderBy(i => i.Foodstuff.Name).ToImmutableList();
            RaisePropertyChanged(nameof(Ingredients));
        }

        private IngredientCellViewModel ToViewModel(Ingredient ingredient)
        {
            return new IngredientCellViewModel(
                ingredient,
                () => IngredientAction(ingredient, i => ShoppingListHandler.IncreaseAmount(i)),
                () => IngredientAction(ingredient, i => ShoppingListHandler.DecreaseAmount(i))
            );
        }
    }
}
