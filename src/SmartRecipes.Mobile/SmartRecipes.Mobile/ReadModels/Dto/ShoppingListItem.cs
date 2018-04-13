using LanguageExt;

namespace SmartRecipes.Mobile
{
    public class ShoppingListItem
    {
        public ShoppingListItem(Some<Foodstuff> foodstuff, Amount amount)
        {
            Foodstuff = foodstuff;
            Amount = amount;
        }

        public Some<Foodstuff> Foodstuff { get; }

        public Amount Amount { get; }
    }
}
