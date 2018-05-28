using System;

namespace SmartRecipes.Mobile.Models
{
    public interface IFoodstuff : IEquatable<IFoodstuff>
    {
        Guid Id { get; }

        string Name { get; }

        Uri ImageUrl { get; }

        IAmount BaseAmount { get; }

        IAmount AmountStep { get; }
    }
}
