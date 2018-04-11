using System;
using SmartRecipes.Mobile.Models;

namespace SmartRecipes.Mobile
{
    public class SignInCommand
    {
        public SignInCommand(SignInCredentials credentials)
        {
            Credentials = credentials;
        }

        public SignInCredentials Credentials { get; }
    }
}
