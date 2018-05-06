using System.Linq;
using System.Collections.Generic;
using SmartRecipes.Mobile.ReadModels;
using SmartRecipes.Mobile.WriteModels;
using SmartRecipes.Mobile.Models;
using System.Collections.Immutable;

namespace SmartRecipes.Mobile.ViewModels
{
    public class FoodstuffSearchViewModel : ViewModel
    {
        private readonly ShoppingListHandler commandHandler;

        private readonly FoodstuffRepository repository;

        public FoodstuffSearchViewModel(ShoppingListHandler commandHandler, FoodstuffRepository repository)
        {
            this.repository = repository;
            this.commandHandler = commandHandler;
            SearchResult = ImmutableList.Create<FoodstuffSearchCellViewModel>();
            Selected = ImmutableList.Create<IFoodstuff>();
        }

        public IEnumerable<FoodstuffSearchCellViewModel> SearchResult { get; private set; }

        public IImmutableList<IFoodstuff> Selected { get; set; }

        public async void Search(string query)
        {
            var searchResult = await repository.Search(query);
            SearchResult = searchResult.Select(f => new FoodstuffSearchCellViewModel(f, () => Add(f))); // TODO: separate search model from ingredient view model
            RaisePropertyChanged(nameof(SearchResult));
        }

        private void Add(IFoodstuff foodstuff)
        {
            Selected = Selected.Add(foodstuff);
        }
    }
}
