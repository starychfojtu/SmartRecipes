using SmartRecipes.Mobile.Infrastructure;
using Xamarin.Forms;

namespace SmartRecipes.Mobile.Pages
{
    public class AppContainer : MasterDetailPage
    {
        public AppContainer(
            ShoppingListItemsPage shoppingListItemsPage,
            ShoppingListRecipesPage shoppingListRecipesPage,
            MyRecipesPage myRecipesPage)
        {
            NavigationPage.SetHasNavigationBar(this, false);

            var pages = new Page[]
            {
                PageFactory.CreateTabbed("Shopping list", shoppingListItemsPage, shoppingListRecipesPage),
                PageFactory.CreateTabbed("Recipes", myRecipesPage)
            };

            Detail = pages[0];
            Master = new NavigationDrawer(pages, index =>
            {
                IsPresented = false;
                Detail = pages[index];
            });
        }
    }
}
