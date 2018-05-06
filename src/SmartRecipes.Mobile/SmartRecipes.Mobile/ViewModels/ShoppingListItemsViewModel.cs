using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SmartRecipes.Mobile.ReadModels;
using System;
using SmartRecipes.Mobile.WriteModels;
using SmartRecipes.Mobile.ReadModels.Dto;
using SmartRecipes.Mobile.Services;
using System.Collections.Immutable;

namespace SmartRecipes.Mobile.ViewModels
{
    public class ShoppingListItemsViewModel : ViewModel
    {
        private readonly ShoppingListHandler commandHandler;

        private readonly ShoppingListRepository repository;

        private IImmutableList<Ingredient> items { get; set; }

        public ShoppingListItemsViewModel(ShoppingListHandler commandHandler, ShoppingListRepository repository)
        {
            this.repository = repository;
            this.commandHandler = commandHandler;
            items = ImmutableList.Create<Ingredient>();
        }

        public IEnumerable<FoodstuffCellViewModel> Items
        {
            get { return items.Select(i => ToViewModel(i)); }
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
            await Navigation.OpenAddIngredientDialog();
        }

        private async Task IncreaseAmountAsync(Ingredient item)
        {
            await ItemAction(item, i => commandHandler.IncreaseAmount(i));
        }

        private async Task DecreaseAmountAsync(Ingredient item)
        {
            await ItemAction(item, i => commandHandler.DecreaseAmount(i));
        }

        private async Task ItemAction(Ingredient item, Func<Ingredient, Task<Ingredient>> action)
        {
            var newItem = await action(item);
            var oldItem = items.First(i => i.Foodstuff.Id == item.Foodstuff.Id);
            var newItems = items.Replace(oldItem, newItem);
            UpdateItems(newItems);
        }

        private async Task UpdateItemsAsync()
        {
            UpdateItems((await repository.GetItems()).ToImmutableList());
        }

        private void UpdateItems(IImmutableList<Ingredient> newItems)
        {
            items = newItems.OrderBy(i => i.Foodstuff.Name).ToImmutableList();
            RaisePropertyChanged(nameof(Items));
        }

        private FoodstuffCellViewModel ToViewModel(Ingredient item)
        {
            return new FoodstuffCellViewModel(
                item.Foodstuff,
                item.Amount,
                () => IncreaseAmountAsync(item),
                () => DecreaseAmountAsync(item)
            );
        }
    }
}
