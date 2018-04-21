using SmartRecipes.Mobile.Models;
using SmartRecipes.Mobile.Services;
using SmartRecipes.Mobile.WriteModels;

namespace SmartRecipes.Mobile.ViewModels
{
    public class SignInViewModel : ViewModel
    {
        private readonly SecurityHandler commandHandler;

        public SignInViewModel(SecurityHandler commandHandler)
        {
            this.commandHandler = commandHandler;
        }

        public string Email { get; set; }

        public string Password { get; set; }

        public string Error { get; private set; }

        public async void SignIn()
        {
            Error = await commandHandler.SignIn(new SignInCredentials(Email, Password));
            RaisePropertyChanged(nameof(Error));
        }

        public async void SignUp()
        {
            await Navigation.SignUp(this);
        }
    }
}
