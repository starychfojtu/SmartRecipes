using System;
using System.Threading.Tasks;
using LanguageExt;
using SmartRecipes.Mobile.ApiDto;
using SmartRecipes.Mobile.Models;
using SmartRecipes.Mobile.Services;
using static LanguageExt.Prelude;

namespace SmartRecipes.Mobile.WriteModels
{
    public class SecurityHandler
    {
        private readonly ApiClient apiClient;

        public SecurityHandler(ApiClient apiClient)
        {
            this.apiClient = apiClient;
        }

        public async Task<bool> SignIn(SignInCredentials credentials)
        {
            var result = await Authenticate(credentials, apiClient.Post);

            // TODO: Remove this
            await Navigation.LogIn(this);
            return true;

            //if (result.Success)
            //{
            //    await Navigation.LogIn(this);
            //    return true;
            //}

            //return false;
        }

        public static async Task<AuthenticationResult> Authenticate(SignInCredentials credentials, Func<SignInRequest, Task<Option<SignInResponse>>> post)
        {
            var response = await post(new SignInRequest(credentials.Email, credentials.Password));
            return response.Match(
                r => new AuthenticationResult(r.IsAuthorized, r.Token),
                () => new AuthenticationResult(success: false, token: null)
            );
        }
    }

    public class AuthenticationResult
    {
        public AuthenticationResult(bool success, string token)
        {
            Success = success;
            Token = Optional(token);
        }

        public bool Success { get; }

        public Option<string> Token { get; }
    }
}
