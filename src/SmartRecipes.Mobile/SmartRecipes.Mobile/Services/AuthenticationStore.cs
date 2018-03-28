using LanguageExt;
using SmartRecipes.Mobile.Models;

namespace SmartRecipes.Mobile
{
    public partial class Store
    {
        private Option<string> token;

        public Authenticator.AuthenticationResult Authenticate(SignInCredentials credentials)
        {
            var result = Authenticator.Authenticate(credentials, apiClient.PostSignIn); // TODO : introduce TEE monad
            token = result.Token;
            return result;
        }
    }
}
