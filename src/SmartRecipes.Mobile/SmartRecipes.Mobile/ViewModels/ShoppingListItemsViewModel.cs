using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SmartRecipes.Mobile.Controllers;
using System.Runtime.CompilerServices;

namespace SmartRecipes.Mobile
{
    public class ShoppingListItemsViewModel : ViewModel
    {
        private readonly ShoppingListController controller;

        public ShoppingListItemsViewModel(ShoppingListController controller)
        {
            this.controller = controller;
            GetItems().ContinueWith(t =>
            {
                Items = t.Result;
                RaisePropertyChanged(nameof(Items));
            });
        }

        public IEnumerable<FoodstuffCellViewModel> Items { get; private set; }

        public async Task<IEnumerable<FoodstuffCellViewModel>> GetItems()
        {
            return (await controller.GetItems()).Select(item => FoodstuffCellViewModel.Create(item.Foodstuff, item.Amount, IncreaseItemAmount(item), DecreaseItemAmount(item)));
        }

        private async Task IncreaseItemAmount(Ingredient item)
        {
            await controller.IncreaseAmount(item);
            RaisePropertyChanged(nameof(Items));
        }

        private async Task DecreaseItemAmount(Ingredient item)
        {
            await acontroller.DecreaseAmount(item);
            RaisePropertyChanged(nameof(Items));§
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
