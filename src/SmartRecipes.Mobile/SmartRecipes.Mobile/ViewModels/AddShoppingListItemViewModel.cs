using System.Linq;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using SmartRecipes.Mobile.ReadModels;
using SmartRecipes.Mobile.WriteModels;
using SmartRecipes.Mobile.Models;

namespace SmartRecipes.Mobile.ViewModels
{
    public class AddShoppingListItemViewModel : ViewModel
    {
        private readonly ShoppingListHandler commandHandler;

        private readonly ShoppingListRepository repository;

        public AddShoppingListItemViewModel(ShoppingListHandler commandHandler, ShoppingListRepository repository)
        {
            this.repository = repository;
            this.commandHandler = commandHandler;
            SearchResult = new List<FoodstuffCellViewModel>();
        }

        public IEnumerable<FoodstuffCellViewModel> SearchResult { get; private set; }

        public async void Search(string query)
        {
            SearchResult = (await repository.Search(query)).Select(f => new FoodstuffCellViewModel(f, f.BaseAmount, () => Add(f)));//Add(f)
            RaisePropertyChanged(nameof(SearchResult));
        }

        private async Task Add(Foodstuff foodstuff)
        {
            await commandHandler.Add(foodstuff);
            RaisePropertyChanged(nameof(SearchResult));
        }
    }
}
