using System;

namespace SmartRecipes.Mobile.Models
{
    public interface IAccount
    {
        Guid Id { get; }

        string Email { get; }
    }
}
