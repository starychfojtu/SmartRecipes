using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SmartRecipes.Mobile
{
    public static class Validation
    {
        public static IEnumerable<string> NotEmpty(string input)
        {
            if (_Empty(input))
            {
                yield return "Cannot be empty";
            }
        }

        public static IEnumerable<string> NonEmptyEmail(string input)
        {
            if (_Empty(input))
            {
                yield return "Email cannot be empty";
            }

            if (!_IsEmail(input))
            {
                yield return "Input is not valid email";
            }
        }

        private static bool _Empty(string input)
        {
            return string.IsNullOrEmpty(input);
        }

        private static bool _NotEmpty(string input)
        {
            return !_Empty(input);
        }

        private static bool _IsEmail(string email)
        {
            string validEmailPattern = @"^(?!\.)(""([^""\r\\]|\\[""\r\\])*""|"
            + @"([-a-z0-9!#$%&'*+/=?^_`{|}~]|(?<!\.)\.)*)(?<!\.)"
            + @"@[a-z0-9][\w\.-]*[a-z0-9]\.[a-z][a-z\.]*[a-z]$";

            var regex = new Regex(validEmailPattern, RegexOptions.IgnoreCase);
            return regex.IsMatch(email);
        }
    }
}
