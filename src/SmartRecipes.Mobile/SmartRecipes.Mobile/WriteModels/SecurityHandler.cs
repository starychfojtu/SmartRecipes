using System;
using System.Threading.Tasks;
using LanguageExt;
using SmartRecipes.Mobile.ApiDto;
using SmartRecipes.Mobile.Models;
using static LanguageExt.Prelude;

namespace SmartRecipes.Mobile.Controllers
{
    public class SecurityHandler
    {
        private readonly ApiClient apiClient;

        public SecurityHandler(ApiClient apiClient)
        {
            this.apiClient = apiClient;
        }

        public async Task<string> Handle(SignInCommand command)
        {
            var result = await Authenticate(command.Credentials, apiClient.Post);

            if (result.Success)
            {
                await Navigation.LogIn(this);
            }

            return "Invalid credenatials";
        }

        public static async Task<AuthenticationResult> Authenticate(SignInCredentials credentials, Func<SignInRequest, Task<SignInResponse>> post)
        {
            var response = await post(new SignInRequest(credentials.Email, credentials.Password));
            return new AuthenticationResult(response.IsAuthorized, response.Token);
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
