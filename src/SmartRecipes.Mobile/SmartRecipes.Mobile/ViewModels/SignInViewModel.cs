using SmartRecipes.Mobile.Models;
using SmartRecipes.Mobile.WriteModels;
using System.Threading.Tasks;
using SmartRecipes.Mobile.Infrastructure;

namespace SmartRecipes.Mobile.ViewModels
{
    public class SignInViewModel : ViewModel
    {
        private readonly Enviroment enviroment;

        public SignInViewModel(Enviroment enviroment)
        {
            this.enviroment = enviroment;
            Email = ValidatableObject.Create<string>(
                s => Validation.NotEmpty(s) && Validation.IsEmail(s),
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

        public async Task<bool> SignIn()
        {
            if (FormIsValid)
            {
                var credentials = new SignInCredentials(Email.Data, Password.Data);
                var authResult = await UserHandler.SignIn(enviroment.Api, credentials);

                if (authResult.Success)
                {
                    await enviroment.Db.Seed();
                    await Navigation.LogIn();
                    return true;
                }
            }
            return false;
        }

        public async Task SignUp()
        {
            await Navigation.SignUp();
        }
    }
}
