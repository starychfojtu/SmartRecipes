using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SmartRecipes.Mobile.Controllers;

namespace SmartRecipes.Mobile
{
    public class ShoppingListItemsViewModel : ViewModel
    {
        private readonly ShoppingListController controller;

        private IEnumerable<FoodstuffCellViewModel> items;

        public ShoppingListItemsViewModel(ShoppingListController controller)
        {
            this.controller = controller;
            items = Enumerable.Empty<FoodstuffCellViewModel>();
        }

        public IEnumerable<FoodstuffCellViewModel> Items
        {
            get { return Items.Tee(_ => UpdateItems()); }
        }

        public async Task UpdateItems()
        {
            items = (await controller.GetItems()).Select(item => FoodstuffCellViewModel.Create(item.Foodstuff, item.Amount, IncreaseItemAmount(item), DecreaseItemAmount(item)));
            RaisePropertyChanged(nameof(Items));
        }

        private async Task IncreaseItemAmount(Ingredient item)
        {
            await controller.IncreaseAmount(item);
            RaisePropertyChanged(nameof(Items));
        }

        private async Task DecreaseItemAmount(Ingredient item)
        {
            await acontroller.DecreaseAmount(item);
            RaisePropertyChanged(nameof(Items));
        }

        public void Refresh()
        {
            RaisePropertyChanged(nameof(Items));
        }

        public void NavigateToAddItemPage()
        {
            Navigation.AddShoppingListItem(this);
        }
    }
}
