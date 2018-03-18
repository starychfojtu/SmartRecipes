namespace SmartRecipes.Mobile
{
    public class AddShoppingListItemViewModel
    {
        private readonly SearchEngine searchEngine;

        private readonly ShoppingList shoppingList;

        public AddShoppingListItemViewModel(SearchEngine searchEngine, ShoppingList shoppingList)
        {
            this.shoppingList = shoppingList;
            this.searchEngine = searchEngine;
        }
    }
}
