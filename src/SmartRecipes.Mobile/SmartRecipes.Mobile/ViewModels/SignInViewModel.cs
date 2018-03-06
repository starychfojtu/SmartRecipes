using SmartRecipes.Mobile.Models;
using Xamarin.Forms;
using Autofac;
using SmartRecipes.Mobile.Pages;

namespace SmartRecipes.Mobile
{
    public class SignInViewModel
    {
        private readonly INavigation navigation;

        private readonly Authenticator authenticator;

        public SignInViewModel(Authenticator authenticator)
        {
            this.authenticator = authenticator;
        }

        public SignInCredentials Credentials { get; set; }

        public void SignIn()
        {
            authenticator.Authenticate(
                Credentials,
                () => { },
                () => { });
        }

        public void NavigateToSignUp()
        {
            Application.Current.MainPage = DIContainer.Instance.Resolve<SignUpPage>();
        }
    }
}
