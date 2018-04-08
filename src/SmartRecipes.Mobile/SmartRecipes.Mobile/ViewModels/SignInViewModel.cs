using SmartRecipes.Mobile.Models;
using SmartRecipes.Mobile.Controllers;

namespace SmartRecipes.Mobile
{
    public class SignInViewModel : ViewModel
    {
        private readonly SecurityController controller;

        public SignInViewModel(SecurityController controller)
        {
            this.controller = controller;
        }

        public string Email { get; set; }

        public string Password { get; set; }

        public string Error { get; private set; }

        public async void SignIn()
        {
            Error = await controller.Authenticate(new SignInCredentials(Email, Password));
            RaisePropertyChanged(nameof(Error));
        }

        public void SignUp()
        {
            controller.SignUp();
        }
    }
}
