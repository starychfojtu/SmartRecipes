using System.Linq;
using System.Collections.Generic;
using System;
using SmartRecipes.Mobile.Controllers;
using System.Threading.Tasks;
using SmartRecipes.Mobile.ReadModels;

namespace SmartRecipes.Mobile
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
            SearchResult = (await repository.Search(query)).Select(f => new FoodstuffCellViewModel(f, f.BaseAmount, () => Add(f))); // TODO: add await
            RaisePropertyChanged(nameof(SearchResult));
        }

        private async Task Add(Foodstuff foodstuff)
        {
            await commandHandler.Handle(new AddToShoppingList(foodstuff));
            RaisePropertyChanged(nameof(SearchResult));
        }
    }
}
