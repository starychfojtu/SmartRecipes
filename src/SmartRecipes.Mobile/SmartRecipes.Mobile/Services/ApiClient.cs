using System.Net.Http;

namespace SmartRecipes.Mobile
{
    public class ApiClient
    {
        private readonly HttpClient client;

        private string authenticationToken;

        public ApiClient(HttpClient client)
        {
            this.client = client;
        }

        public SignInResponse PostSignIn(SignInRequest request)
        {
            return new SignInResponse(true, "fakeToken");
        }

        public SignUpResponse PostSignUp(SignUpRequest request)
        {
            return new SignUpResponse(new SignUpResponse.Account("fake@gmail.com"));
        }
    }
}
