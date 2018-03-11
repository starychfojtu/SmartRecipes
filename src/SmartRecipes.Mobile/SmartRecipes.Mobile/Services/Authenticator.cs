using System;
using SmartRecipes.Mobile.ApiDto;
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

        public void Authenticate(SignInCredentials credentials, Action onSuccess, Action onFailure)
        {
            var request = new SignInRequest(credentials.Email, credentials.Password);
            var response = client.PostSignIn(request);

            if (response.IsAuthorized)
            {
                onSuccess();
            }
            else
            {
                onFailure();
            }
        }
    }
}
