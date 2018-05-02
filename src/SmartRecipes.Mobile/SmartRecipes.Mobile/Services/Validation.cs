using System.Text.RegularExpressions;

namespace SmartRecipes.Mobile
{
    public static class Validation
    {
        public static bool Empty(string input)
        {
            return string.IsNullOrEmpty(input);
        }

        public static bool NotEmpty(string input)
        {
            return !Empty(input);
        }

        public static bool IsEmail(string email)
        {
            string validEmailPattern = @"^(?!\.)(""([^""\r\\]|\\[""\r\\])*""|"
            + @"([-a-z0-9!#$%&'*+/=?^_`{|}~]|(?<!\.)\.)*)(?<!\.)"
            + @"@[a-z0-9][\w\.-]*[a-z0-9]\.[a-z][a-z\.]*[a-z]$";
            var regex = new Regex(validEmailPattern, RegexOptions.IgnoreCase);

            return regex.IsMatch(email);
        }
    }
}
