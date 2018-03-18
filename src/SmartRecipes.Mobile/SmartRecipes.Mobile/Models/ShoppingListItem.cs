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

        public Amount Amount { get; set; }
    }
}
