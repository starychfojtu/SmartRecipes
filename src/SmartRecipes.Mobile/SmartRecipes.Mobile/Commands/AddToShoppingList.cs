using System;

namespace SmartRecipes.Mobile
{
    public class AddToShoppingList
    {
        public AddToShoppingList(Foodstuff foodstuff)
        {
            Foodstuff = foodstuff;
        }

        public Foodstuff Foodstuff { get; }
    }
}
