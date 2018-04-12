using System.Linq;
using System;
using LanguageExt;
using static LanguageExt.Prelude;

namespace SmartRecipes.Mobile
{
    public class Ingredient
    {
        private Ingredient(Foodstuff foodstuff, Amount amount)
        {
            Foodstuff = foodstuff;
            Amount = amount;
        }

        public Ingredient() { /* for sqllite */ }

        public Foodstuff Foodstuff { get; }

        public Amount Amount { get; }

        public Ingredient WithAmount(Amount amount)
        {
            return new Ingredient(Foodstuff, amount);
        }

        // Combinators

        public static Ingredient Create(Foodstuff foodstuff, Amount amount)
        {
            return new Ingredient(foodstuff, amount);
        }

        public static Ingredient Create(Foodstuff foodstuff)
        {
            return new Ingredient(foodstuff, foodstuff.BaseAmount);
        }

        public static Option<Ingredient> IncreaseAmount(Ingredient item)
        {
            return ChangeAmount(item, Amount.Add);
        }

        public static Option<Ingredient> DecreaseAmount(Ingredient item)
        {
            var newItem = ChangeAmount(item, Amount.Substract);
            return newItem.Bind(i => Amount.IsLessThanOrEquals(i.Amount, Amount.Zero(i.Amount.Unit)) ? None : Some(i));
        }

        public static Option<Ingredient> ChangeAmount(Ingredient item, Func<Amount, Amount, Option<Amount>> operation)
        {
            var changedAmount = operation(item.Amount, item.Foodstuff.AmountStep);
            return changedAmount.Map(a => item.WithAmount(a));
        }
    }
}
