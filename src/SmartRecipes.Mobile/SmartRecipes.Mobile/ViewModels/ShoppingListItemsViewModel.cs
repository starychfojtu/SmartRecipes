using System.Collections.Generic;

namespace SmartRecipes.Mobile
{
    public class ShoppingListItemsViewModel
    {
        private readonly ShoppingList shoppingList;

        public ShoppingListItemsViewModel(ShoppingList shoppingList)
        {
            this.shoppingList = shoppingList;
        }

        public IEnumerable<ShoppingListItem> Items
        {
            get { return shoppingList.GetItems(); }
        }
    }
}
