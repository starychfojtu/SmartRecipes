using Xamarin.Forms;
using Autofac;
using SmartRecipes.Mobile.Pages;
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

        public IEnumerable<ShoppingListItemCellViewModel> Items
        {
            get { return store.ShoppingListItems.Select(i => new ShoppingListItemCellViewModel(i, store)); }
        }

        public void NavigateToAddItemPage()
        {
            Application.Current.MainPage.Navigation.PushAsync(DIContainer.Instance.Resolve<AddShoppingListItemPage>());
        }
    }
}
