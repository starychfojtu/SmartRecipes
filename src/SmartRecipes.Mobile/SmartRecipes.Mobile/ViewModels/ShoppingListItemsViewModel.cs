using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SmartRecipes.Mobile.ReadModels;
using System;
using SmartRecipes.Mobile.WriteModels;
using SmartRecipes.Mobile.ReadModels.Dto;
using SmartRecipes.Mobile.Services;

namespace SmartRecipes.Mobile.ViewModels
{
    public class ShoppingListItemsViewModel : ViewModel
    {
        private readonly ShoppingListHandler commandHandler;

        private readonly ShoppingListRepository repository;

        private IList<Ingredient> items { get; set; }

        public ShoppingListItemsViewModel(ShoppingListHandler commandHandler, ShoppingListRepository repository)
        {
            this.repository = repository;
            this.commandHandler = commandHandler;
            items = new List<Ingredient>();
        }

        public IEnumerable<FoodstuffCellViewModel> Items
        {
            get { return items.Select(i => ToViewModel(i)); }
        }

        public override async Task InitializeAsync()
        {
            await UpdateItemsAsync();
        }

        public void Refresh()
        {
            RaisePropertyChanged(nameof(Items));
        }

        public async Task AddItem()
        {
            await Navigation.AddShoppingListItem(this);
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
            items.Replace(oldItem, newItem);
            UpdateItems(items);
        }

        private async Task UpdateItemsAsync()
        {
            UpdateItems((await repository.GetItems()).ToList());
        }

        private void UpdateItems(IList<Ingredient> newItems)
        {
            items = newItems;
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
