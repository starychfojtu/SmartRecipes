using System.Linq;
using System.Collections.Generic;
using System;

namespace SmartRecipes.Mobile
{
    public class AddShoppingListItemViewModel : ViewModel
    {
        public AddShoppingListItemViewModel(Store store) : base(store)
        {
            SearchResult = new List<FoodstuffCellViewModel>();
        }

        public IEnumerable<FoodstuffCellViewModel> SearchResult { get; private set; }

        public void Search(string query)
        {
            SearchResult = store.Search(query).Select(f => FoodstuffCellViewModel.Create(f, f.BaseAmount, Add(f)));
            RaisePropertyChanged(nameof(SearchResult));
        }

        private Action Add(Foodstuff foodstuff)
        {
            return () =>
            {
                store.Add(foodstuff);
                RaisePropertyChanged(nameof(SearchResult));
            };
        }
    }
}
