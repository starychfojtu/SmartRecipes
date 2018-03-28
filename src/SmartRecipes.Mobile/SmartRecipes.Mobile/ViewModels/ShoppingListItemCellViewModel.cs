namespace SmartRecipes.Mobile
{
    public class ShoppingListItemCellViewModel
    {
        public ShoppingListItemCellViewModel(ShoppingListItem item, Store store)
        {
            Item = item;
            Store = store;
        }

        public ShoppingListItem Item { get; }

        public Store Store { get; }
    }
}
