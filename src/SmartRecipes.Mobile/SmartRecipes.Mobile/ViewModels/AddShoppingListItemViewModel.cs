using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using SmartRecipes.Mobile.ReadModels;
using SmartRecipes.Mobile.WriteModels;
using SmartRecipes.Mobile.Models;
using System.Collections.Immutable;

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
            SearchResult = ImmutableList.Create<FoodstuffCellViewModel>();
        }

        public IEnumerable<FoodstuffCellViewModel> SearchResult { get; private set; }

        public async void Search(string query)
        {
            var searchResult = await repository.Search(query);
            SearchResult = searchResult.Select(f => new FoodstuffCellViewModel(f, f.BaseAmount, () => Add(f)));
            RaisePropertyChanged(nameof(SearchResult));
        }

        private async Task Add(IFoodstuff foodstuff)
        {
            await commandHandler.Add(foodstuff);
            RaisePropertyChanged(nameof(SearchResult));
        }
    }
}
