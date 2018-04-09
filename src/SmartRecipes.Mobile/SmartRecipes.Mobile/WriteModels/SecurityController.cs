using System.Threading.Tasks;
using SmartRecipes.Mobile.Models;
using static SmartRecipes.Mobile.Authenticator;

namespace SmartRecipes.Mobile.Controllers
{
    public class SecurityController
    {
        private readonly ApiClient apiClient;

        public SecurityController(ApiClient apiClient)
        {
            this.apiClient = apiClient;
        }

        public async Task<string> Authenticate(SignInCredentials credentials)
        {
            var result = await Authenticator.Authenticate(credentials, apiClient.Post);

            if (result.Success)
            {
                Navigation.LogIn(this);
            }

            return "Invalid credenatials";
        }

        public void SignUp()
        {
            Navigation.SignUp(this);
        }
    }
}
