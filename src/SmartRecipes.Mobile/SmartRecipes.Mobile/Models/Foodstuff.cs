using System;
using SQLite;

namespace SmartRecipes.Mobile
{
    public class Foodstuff
    {
        private Foodstuff(Guid id, string name, Uri imageUrl, Amount baseAmount, Amount amountStep)
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

        public string _ImageUrlString { get; set; }
        [Ignore]
        public Uri ImageUrl
        {
            get { return new Uri(_ImageUrlString); }
            set { _ImageUrlString = value.AbsoluteUri; }
        }

        public int _BaseCount { get; set; }
        public AmountUnit _BaseUnit { get; set; }
        [Ignore]
        public Amount BaseAmount
        {
            get { return new Amount(_BaseCount, _BaseUnit); }
            set
            {
                _BaseCount = value.Count;
                _BaseUnit = value.Unit;
            }
        }

        public int _StepCount { get; set; }
        public AmountUnit _StepUnit { get; set; }
        [Ignore]
        public Amount AmountStep
        {
            get { return new Amount(_StepCount, _StepUnit); }
            set
            {
                _StepCount = value.Count;
                _StepUnit = value.Unit;
            }
        }

        public bool Equals(Foodstuff foodstuff)
        {
            return Id == foodstuff.Id;
        }

        // 

        public static Foodstuff Create(Guid id, string name, Uri imageUrl, Amount baseAmount, Amount amountStep)
        {
            return new Foodstuff(id, name, imageUrl, baseAmount, amountStep);
        }
    }
}
