using System.Collections.Generic;
using Xamarin.Forms;
using Autofac;
using SmartRecipes.Mobile.Pages;

namespace SmartRecipes.Mobile
{
    public class ShoppingListItemsViewModel
    {
        private readonly ShoppingList shoppingList;

        public ShoppingListItemsViewModel(ShoppingList shoppingList)
        {
            this.shoppingList = shoppingList;
        }

        public IEnumerable<ShoppingListItem> Items
        {
            get { return shoppingList.GetItems(); }
        }

        public void NavigateToAddItemPage()
        {
            Application.Current.MainPage.Navigation.PushAsync(DIContainer.Instance.Resolve<AddShoppingListItemPage>());
        }
    }
}
