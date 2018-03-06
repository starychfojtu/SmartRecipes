namespace SmartRecipes.Mobile.Models
{
    public class SignInCredentials
    {
        public SignInCredentials(string email, string password)
        {
            Email = email;
            Password = password;
        }

        public string Email { get; set; }

        public string Password { get; set; }
    }
}
