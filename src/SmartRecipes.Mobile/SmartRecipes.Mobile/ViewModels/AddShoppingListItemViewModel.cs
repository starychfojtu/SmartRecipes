using System.Collections.ObjectModel;

namespace SmartRecipes.Mobile
{
    public class AddShoppingListItemViewModel
    {
        private readonly Store store;

        public AddShoppingListItemViewModel(Store store)
        {
            this.store = store;
            SearchResult = new ObservableCollection<ShoppingListItem>();
        }

        public ObservableCollection<ShoppingListItem> SearchResult { get; private set; }

        public void Search(string query)
        {
            var searchResult = store.Search(query);
            SearchResult.Clear();
            foreach (var item in searchResult)
            {
                SearchResult.Add(item);
            }
        }
    }
}
