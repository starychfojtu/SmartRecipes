using System;
using SQLite;

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

        public Foodstuff() { /* for sqlite */ }

        [PrimaryKey]
        public Guid Id { get; }

        public string Name { get; }

        public Uri ImageUrl { get; }

        public Amount BaseAmount { get; }

        public Amount AmountStep { get; }

        public bool Equals(Foodstuff foodstuff)
        {
            return Id == foodstuff.Id;
        }
    }
}
