using SmartRecipes.Mobile.Models;
using Xamarin.Forms;
using Autofac.Core;
using Autofac;
using SmartRecipes.Mobile.Pages;

namespace SmartRecipes.Mobile
{
    public class SignInViewModel
    {
        private readonly INavigation navigation;

        public SignInViewModel(INavigation navigation)
        {
            this.navigation = navigation;
        }

        public SignInCredentials Credentials { get; set; }

        public void SignIn()
        {

        }

        public void NavigateToSignUp()
        {
            Application.Current.MainPage = DIContainer.Instance.Resolve<SignUpPage>();
        }
    }
}
