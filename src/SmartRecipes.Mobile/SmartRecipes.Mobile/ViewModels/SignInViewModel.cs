using SmartRecipes.Mobile.Models;
using SmartRecipes.Mobile.Services;
using SmartRecipes.Mobile.WriteModels;
using System.Linq;

namespace SmartRecipes.Mobile.ViewModels
{
    public class SignInViewModel : ViewModel
    {
        private readonly SecurityHandler commandHandler;

        public SignInViewModel(SecurityHandler commandHandler)
        {
            this.commandHandler = commandHandler;
            Email = new ValidatableObject<string>(
                "",
                s => s.Contains("@") ? Enumerable.Empty<string>() : new[] { "Fuck off" },
                _ => RaisePropertyChanged(nameof(Email))
            );
        }

        public ValidatableObject<string> Email { get; set; }

        public string Password { get; set; }

        public string Error { get; private set; }

        public async void SignIn()
        {
            Error = await commandHandler.SignIn(new SignInCredentials(Email.Data, Password));
            RaisePropertyChanged(nameof(Error));
        }

        public async void SignUp()
        {
            await Navigation.SignUp(this);
        }
    }
}
