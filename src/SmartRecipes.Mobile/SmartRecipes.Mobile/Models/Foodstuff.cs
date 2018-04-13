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
        public Guid Id { get; set; }

        public string Name { get; set; }

        public Uri ImageUrl { get; set; }

        public Amount BaseAmount { get; set; }

        public Amount AmountStep { get; set; }

        public bool Equals(Foodstuff foodstuff)
        {
            return Id == foodstuff.Id;
        }
    }
}
