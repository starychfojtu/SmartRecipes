using System.Linq;
using System;
using LanguageExt;
using static LanguageExt.Prelude;

namespace SmartRecipes.Mobile
{
    public class ShoppingListItem
    {
        private ShoppingListItem(Foodstuff foodstuff, Amount amount)
        {
            Foodstuff = foodstuff;
            Amount = amount;
        }

        public Foodstuff Foodstuff { get; }

        public Amount Amount { get; }

        public ShoppingListItem WithAmount(Amount amount)
        {
            return new ShoppingListItem(Foodstuff, amount);
        }

        // Combinators

        public static ShoppingListItem Create(Foodstuff foodstuff, Amount amount)
        {
            return new ShoppingListItem(foodstuff, amount);
        }

        public static ShoppingListItem Create(Foodstuff foodstuff)
        {
            return new ShoppingListItem(foodstuff, foodstuff.BaseAmount);
        }

        public static Option<ShoppingListItem> IncreaseAmount(ShoppingListItem item)
        {
            return ChangeAmount(item, Amount.Add);
        }

        public static Option<ShoppingListItem> DecreaseAmount(ShoppingListItem item)
        {
            var newItem = ChangeAmount(item, Amount.Substract);
            return newItem.Bind(i => Amount.IsLessThanOrEquals(i.Amount, Amount.Zero(i.Amount.Unit)) ? None : Some(i));
        }

        public static Option<ShoppingListItem> ChangeAmount(ShoppingListItem item, Func<Amount, Amount, Option<Amount>> operation)
        {
            var changedAmount = operation(item.Amount, item.Foodstuff.AmountStep);
            return changedAmount.Map(a => item.WithAmount(a));
        }
    }
}
