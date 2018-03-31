using SmartRecipes.Mobile.Models;
using Xamarin.Forms;
using Autofac;
using SmartRecipes.Mobile.Pages;

namespace SmartRecipes.Mobile
{
    public class SignInViewModel : ViewModel
    {
        public SignInViewModel(Store store) : base(store)
        {
        }

        public string Email { get; set; }

        public string Password { get; set; }

        public void SignIn()
        {
            var result = store.Authenticate(new SignInCredentials(Email, Password));
            if (result.Success)
            {
                Navigation.LogIn(this);
            }
        }

        public void NavigateToSignUp()
        {
            Navigation.SignUp(this);
        }
    }
}
