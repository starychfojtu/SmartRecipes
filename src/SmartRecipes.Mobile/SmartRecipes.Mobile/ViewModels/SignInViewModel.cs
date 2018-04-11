using SmartRecipes.Mobile.Models;
using SmartRecipes.Mobile.Controllers;

namespace SmartRecipes.Mobile
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
            Error = await commandHandler.Handle(new SignInCommand(new SignInCredentials(Email, Password)));
            RaisePropertyChanged(nameof(Error));
        }

        public void SignUp()
        {
            Navigation.SignUp(this);
        }
    }
}
