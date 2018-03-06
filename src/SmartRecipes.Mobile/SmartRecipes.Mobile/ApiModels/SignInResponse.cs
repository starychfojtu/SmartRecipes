namespace SmartRecipes.Mobile
{
    public class SignInResponse
    {
        public SignInResponse(bool isAuthorized, string token)
        {
            IsAuthorized = isAuthorized;
            Token = token;
        }

        public bool IsAuthorized { get; }

        public string Token { get; }
    }
}
