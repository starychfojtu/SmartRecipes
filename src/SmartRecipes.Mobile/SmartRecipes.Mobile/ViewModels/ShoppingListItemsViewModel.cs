using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SmartRecipes.Mobile.Controllers;
using SmartRecipes.Mobile.ReadModels;

namespace SmartRecipes.Mobile
{
    public class ShoppingListItemsViewModel : ViewModel
    {
        private readonly ShoppingListHandler commandHandler;

        private readonly ShoppingListRepository repository;

        private IList<ShoppingListItem> items { get; set; }

        public ShoppingListItemsViewModel(ShoppingListHandler commandHandler, ShoppingListRepository repository)
        {
            this.repository = repository;
            this.commandHandler = commandHandler;
            items = new List<ShoppingListItem>();
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

        private async Task IncreaseAmountAsync(ShoppingListItem item)
        {
            // TODO: resolve async issue when updating - LOCK/ bool isUpdating ?
            var increased = await commandHandler.IncreaseAmount(item);
            UpdateItems(items.Replace(item, increased));
        }

        private async Task DecreaseAmountAsync(ShoppingListItem item)
        {
            var decreased = await commandHandler.DecreaseAmount(item);
            UpdateItems(items.Replace(item, decreased));
        }

        private async Task UpdateItemsAsync()
        {
            UpdateItems((await repository.GetItems()).ToList());
        }

        private void UpdateItems(IList<ShoppingListItem> newItems)
        {
            items = newItems;
            RaisePropertyChanged(nameof(Items));
        }

        private FoodstuffCellViewModel ToViewModel(ShoppingListItem item)
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
