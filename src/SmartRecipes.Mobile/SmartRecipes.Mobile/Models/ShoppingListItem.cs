using System.Linq;
using System;
using LanguageExt;

namespace SmartRecipes.Mobile
{
    public class ShoppingListItem
    {
        public ShoppingListItem(Foodstuff foodstuff, Amount amount)
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

        public static ShoppingListItem IncreaseAmount(ShoppingListItem item)
        {
            return ChangeAmount(item, Amount.Add);
        }

        public static ShoppingListItem DecreaseAmount(ShoppingListItem item)
        {
            return ChangeAmount(item, Amount.Substract);
        }

        public static ShoppingListItem ChangeAmount(ShoppingListItem item, Func<Amount, Amount, Option<Amount>> operation)
        {
            var changedAmount = operation(item.Amount, item.Foodstuff.AmountStep);
            return item.WithAmount(changedAmount.SingleOrDefault()); // TODO : think about this - refactor
        }
    }
}
