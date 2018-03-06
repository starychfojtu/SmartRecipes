namespace SmartRecipes.Mobile
{
    public class SignUpResponse
    {
        public SignUpResponse(Account account)
        {
            NewAccount = account;
        }

        public Account NewAccount { get; }

        public class Account
        {
            public Account(string email)
            {
                Email = email;
            }

            public string Email { get; }
        }
    }
}
