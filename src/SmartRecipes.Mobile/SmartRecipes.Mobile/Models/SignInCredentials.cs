namespace SmartRecipes.Mobile.Models
{
    public class SignInCredentials
    {
        public SignInCredentials(string email, string password)
        {
            Email = email;
            Password = password;
        }

        public string Email { get; }

        public string Password { get; }
    }
}
