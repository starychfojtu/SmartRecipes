using SmartRecipes.Mobile.Models;
using SmartRecipes.Mobile.Services;
using SmartRecipes.Mobile.WriteModels;
using System.Threading.Tasks;

namespace SmartRecipes.Mobile.ViewModels
{
    public class SignInViewModel : ViewModel
    {
        private readonly SecurityHandler commandHandler;

        public SignInViewModel(SecurityHandler commandHandler)
        {
            this.commandHandler = commandHandler;

            Email = ValidatableObject.Create<string>(
                s => Validation.NonEmptyEmail(s),
                _ => RaisePropertyChanged(nameof(Email))
            );
            Password = ValidatableObject.Create<string>(
                s => Validation.NotEmpty(s),
                _ => RaisePropertyChanged(nameof(Password))
            );
        }

        public ValidatableObject<string> Email { get; set; }

        public ValidatableObject<string> Password { get; set; }

        public bool FormIsValid
        {
            get { return Email.IsValid && Password.IsValid; }
        }

        public async Task<string> SignIn()
        {
            return FormIsValid
                ? await commandHandler.SignIn(new SignInCredentials(Email.Data, Password.Data))
                : "Please fill in valid input";
        }

        public async Task SignUp()
        {
            await Navigation.SignUp(this);
        }
    }
}
