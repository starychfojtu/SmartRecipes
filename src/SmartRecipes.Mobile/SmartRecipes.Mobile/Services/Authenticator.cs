using System;
using SmartRecipes.Mobile.ApiDto;
using SmartRecipes.Mobile.Models;
using LanguageExt;
using static LanguageExt.Prelude;
using System.Threading.Tasks;

namespace SmartRecipes.Mobile
{
    public static class Authenticator
    {
        public static async Task<AuthenticationResult> Authenticate(SignInCredentials credentials, Func<SignInRequest, Task<SignInResponse>> post)
        {
            var response = await post(new SignInRequest(credentials.Email, credentials.Password));
            return new AuthenticationResult(response.IsAuthorized, response.Token);
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
}
