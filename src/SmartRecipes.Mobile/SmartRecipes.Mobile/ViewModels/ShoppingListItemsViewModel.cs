using System.Collections.Generic;
using Xamarin.Forms;
using Autofac;
using SmartRecipes.Mobile.Pages;

namespace SmartRecipes.Mobile
{
    public class ShoppingListItemsViewModel : ViewModel
    {
        public ShoppingListItemsViewModel(Store store) : base(store)
        {
        }

        public IEnumerable<ShoppingListItem> Items
        {
            get { return store.ShoppingListItems; }
        }

        public void NavigateToAddItemPage()
        {
            Application.Current.MainPage.Navigation.PushAsync(DIContainer.Instance.Resolve<AddShoppingListItemPage>());
        }
    }
}
