using System;
namespace SmartRecipes.Mobile
{
    public class Foodstuff
    {
        public Foodstuff(Guid id, string name, Uri imageUrl, Amount baseAmount, Amount amountStep)
        {
            Id = id;
            Name = name;
            ImageUrl = imageUrl;
            BaseAmount = baseAmount;
            AmountStep = amountStep;
        }

        public Guid Id { get; }

        public string Name { get; }

        public Uri ImageUrl { get; }

        public Amount BaseAmount { get; }

        public Amount AmountStep { get; }
    }
}
