using Xamarin.Forms;
using Autofac;
using SmartRecipes.Mobile.Pages;
using SmartRecipes.Mobile.Controllers;
using System.Threading.Tasks;

namespace SmartRecipes.Mobile
{
    public static class Navigation
    {
        public static async Task LogIn(SecurityHandler controller)
        {
            Application.Current.MainPage = new NavigationPage(await PageFactory.GetPageAsync<AppContainer>());
        }

        public static async Task SignUp(SignInViewModel viewModel)
        {
            Application.Current.MainPage = await PageFactory.GetPageAsync<SignUpPage>();
        }

        public static async Task AddShoppingListItem(ShoppingListItemsViewModel viewModel)
        {
            var page = await PageFactory.GetPageAsync<AddShoppingListItemPage>();
            await Application.Current.MainPage.Navigation.PushAsync(page);
        }
    }
}
