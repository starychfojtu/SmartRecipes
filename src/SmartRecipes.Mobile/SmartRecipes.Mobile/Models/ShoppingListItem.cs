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

        public void DecreaseAmount()
        {
            Amount = Amount.Substract(Foodstuff.AmountStep);
        }

        public void IncreaseAmount()
        {
            Amount = Amount.Add(Foodstuff.AmountStep);
        }
    }
}
