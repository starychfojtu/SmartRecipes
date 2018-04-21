using SmartRecipes.Mobile.Services;
using Xamarin.Forms;

namespace SmartRecipes.Mobile.Pages
{
    public class AppContainer : MasterDetailPage
    {
        private Page[] pages;

        public AppContainer(ShoppingListItemsPage shoppingListItemsPage, MyRecipesPage myRecipesPage)
        {
            NavigationPage.SetHasNavigationBar(this, false);

            pages = new Page[]
            {
                PageFactory.CreateTabbed("Shopping list", shoppingListItemsPage),
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
