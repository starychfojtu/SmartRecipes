using System;

namespace SmartRecipes.Mobile.Models
{
    public interface IRecipe
    {
        Guid Id { get; }

        Guid OwnerId { get; }

        string Name { get; }

        Uri ImageUrl { get; }

        int PersonCount { get; }

        string Text { get; }
    }
}
