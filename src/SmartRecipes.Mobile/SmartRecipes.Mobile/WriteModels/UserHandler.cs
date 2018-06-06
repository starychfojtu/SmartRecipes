using System.Threading.Tasks;
using LanguageExt;
using SmartRecipes.Mobile.ApiDto;
using SmartRecipes.Mobile.Models;
using SmartRecipes.Mobile.Services;
using static LanguageExt.Prelude;

namespace SmartRecipes.Mobile.WriteModels
{
    public static class UserHandler
    {
        public static async Task<AuthenticationResult> SignIn(ApiClient apiClient, SignInCredentials credentials)
        {
            var request = new SignInRequest(credentials.Email, credentials.Password);
            var response = await apiClient.Post(request);
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
