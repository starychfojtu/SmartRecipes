using System.Collections.ObjectModel;

namespace SmartRecipes.Mobile
{
    public class AddShoppingListItemViewModel
    {
        public AddShoppingListItemViewModel(Store store)
        {
            SearchResult = new ObservableCollection<ShoppingListItem>();
        }

        public ObservableCollection<ShoppingListItem> SearchResult { get; private set; }
    }
}
