using System.Collections.ObjectModel;

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
            SearchResult = new ObservableCollection<ShoppingListItem>();
        }

        public ObservableCollection<ShoppingListItem> SearchResult { get; private set; }

        public void Search(string query)
        {
            SearchResult.Clear();
            foreach (var f in searchEngine.Search(query))
            {
                SearchResult.Add(new ShoppingListItem(f, f.BaseAmount));
            }
        }
    }
}
