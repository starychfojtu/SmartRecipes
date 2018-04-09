using Xamarin.Forms;
using Autofac;
using SmartRecipes.Mobile.Pages;
using SmartRecipes.Mobile.Controllers;

namespace SmartRecipes.Mobile
{
    public static class Navigation
    {
        public static void LogIn(SecurityController controller)
        {
            Application.Current.MainPage = new NavigationPage(DIContainer.Instance.Resolve<AppContainer>());
        }

        public static void SignUp(SecurityController controller)
        {
            Application.Current.MainPage = DIContainer.Instance.Resolve<SignUpPage>();
        }

        public static void AddShoppingListItem(ShoppingListItemsViewModel viewModel)
        {
            Application.Current.MainPage.Navigation.PushAsync(DIContainer.Instance.Resolve<AddShoppingListItemPage>());
        }
    }
}
