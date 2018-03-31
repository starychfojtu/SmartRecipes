using System;
using System.Collections.Generic;
using System.Linq;

namespace SmartRecipes.Mobile
{
    public class ShoppingListItemsViewModel : ViewModel
    {
        public ShoppingListItemsViewModel(Store store) : base(store)
        {
        }

        public IEnumerable<FoodstuffCellViewModel> Items
        {
            get { return store.ShoppingListItems.Select(item => FoodstuffCellViewModel.Create(item.Foodstuff, item.Amount, IncreaseItemAmount(item), DecreaseItemAmount(item))); }
        }

        private Action IncreaseItemAmount(ShoppingListItem item)
        {
            return () =>
            {
                store.IncreaseAmount(item);
                RaisePropertyChanged(nameof(Items));
            };
        }

        private Action DecreaseItemAmount(ShoppingListItem item)
        {
            return () =>
            {
                store.DecreaseAmount(item);
                RaisePropertyChanged(nameof(Items));
            };
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
