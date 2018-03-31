using System;
using SmartRecipes.Mobile.Models;
using Xamarin.Forms;
using Autofac;
using SmartRecipes.Mobile.Pages;

namespace SmartRecipes.Mobile
{
    public static class Navigation
    {
        public static void LogIn(SignInViewModel viewModel)
        {
            Application.Current.MainPage = new NavigationPage(DIContainer.Instance.Resolve<ShoppingListPage>());
        }

        public static void SignUp(SignInViewModel viewModel)
        {
            Application.Current.MainPage = DIContainer.Instance.Resolve<SignUpPage>();
        }

        public static void AddShoppingListItem(ShoppingListItemsViewModel viewModel)
        {
            Application.Current.MainPage.Navigation.PushAsync(DIContainer.Instance.Resolve<AddShoppingListItemPage>());
        }
    }
}
