using Xamarin.Forms;
using Autofac;

namespace SmartRecipes.Mobile.Pages
{
    public class ShoppingListPage : MasterDetailPage //TabbedPage
    {
        public ShoppingListPage()
        {
            NavigationPage.SetHasNavigationBar(this, false);

            var shoppingListPage = new TabbedPage();
            shoppingListPage.Children.Add(DIContainer.Instance.Resolve<ShoppingListItemsPage>());

            Master = new NavigationDrawer();
            Detail = shoppingListPage;
        }
    }
}
