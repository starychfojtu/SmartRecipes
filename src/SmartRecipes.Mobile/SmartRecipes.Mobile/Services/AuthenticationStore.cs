using LanguageExt;
using SmartRecipes.Mobile.Models;

namespace SmartRecipes.Mobile
{
    public partial class Store
    {
        private Option<string> token;

        public Authenticator.AuthenticationResult Authenticate(SignInCredentials credentials)
        {
            return Authenticator.Authenticate(credentials, apiClient.PostSignIn).Tee(r => token = r.Token);
        }
    }
}
