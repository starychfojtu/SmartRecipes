using SmartRecipes.Mobile.Models;
using Xamarin.Forms;
using Autofac;
using SmartRecipes.Mobile.Pages;

namespace SmartRecipes.Mobile
{
    public class SignInViewModel
    {
        private readonly Store store;

        public SignInViewModel(Store store)
        {
            this.store = store;
        }

        public string Email { get; set; }

        public string Password { get; set; }

        public void SignIn()
        {
            var result = store.Authenticate(new SignInCredentials(Email, Password));
            if (result.Success)
            {
                NavigateToApp();
            }
        }

        public void NavigateToSignUp()
        {
            Application.Current.MainPage = DIContainer.Instance.Resolve<SignUpPage>();
        }

        public void NavigateToApp()
        {
            Application.Current.MainPage = new NavigationPage(DIContainer.Instance.Resolve<ShoppingListPage>());
        }
    }
}
