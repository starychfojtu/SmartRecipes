using System.Net.Http;

namespace SmartRecipes.Mobile
{
    public class ApiClient
    {
        private readonly HttpClient client;

        public ApiClient(HttpClient client)
        {
            this.client = client;
        }

        private string AuthenticationToken { get; set; }

        public SignInResponse PostSignIn(SignInRequest request)
        {
            //if (request.Email == "test@gmail.com" && request.Password == "1234")
            //{
            //AuthenticationToken = "fake";
            return new SignInResponse(true, AuthenticationToken);
            //}

            //return new SignInResponse(false, "");
        }

        public SignUpResponse PostSignUp(SignUpRequest request)
        {
            return new SignUpResponse(new SignUpResponse.Account("fake@gmail.com"));
        }
    }
}
