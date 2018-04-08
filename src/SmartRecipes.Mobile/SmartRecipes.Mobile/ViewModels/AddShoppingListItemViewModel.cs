using System.Linq;
using System.Collections.Generic;
using System;
using SmartRecipes.Mobile.Controllers;
using System.Threading.Tasks;

namespace SmartRecipes.Mobile
{
    public class AddShoppingListItemViewModel : ViewModel
    {
        private readonly ShoppingListController controller;

        public AddShoppingListItemViewModel(ShoppingListController controller)
        {
            Controller = controller;
            SearchResult = new List<FoodstuffCellViewModel>();
        }

        public IEnumerable<FoodstuffCellViewModel> SearchResult { get; private set; }

        public async void Search(string query)
        {
            SearchResult = (await controller.Search(query)).Select(f => FoodstuffCellViewModel.Create(f, f.BaseAmount, () => Add(f))); // TODO: add await
            RaisePropertyChanged(nameof(SearchResult));
        }

        private async Task Add(Foodstuff foodstuff)
        {
            await controller.Add(foodstuff);
            RaisePropertyChanged(nameof(SearchResult));
        }
    }
}
