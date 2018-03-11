using System;
namespace SmartRecipes.Mobile
{
    public class ShoppingListItem
    {
        public ShoppingListItem(Foodstuff foodstuff, int amount)
        {
            Foodstuff = foodstuff;
            Amount = amount;
        }

        public Foodstuff Foodstuff { get; }

        public int Amount { get; set; }
    }
}
