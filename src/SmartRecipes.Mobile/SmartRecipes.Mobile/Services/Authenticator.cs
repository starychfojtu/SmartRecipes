using System;
using SmartRecipes.Mobile.Models;

namespace SmartRecipes.Mobile
{
    public class Authenticator
    {
        private readonly ApiClient client;

        public Authenticator(ApiClient client)
        {
            this.client = client;
        }

        public bool Authenticate(SignInCredentials credentials, Action onFailure)
        {
            var request = new SignInRequest(credentials.Email, credentials.Password);
            var response = client.PostSignIn(request);
            return response.IsAuthorized;
        }
    }
}
