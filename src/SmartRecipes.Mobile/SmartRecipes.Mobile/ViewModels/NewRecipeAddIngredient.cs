using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using SmartRecipes.Mobile.ReadModels;
using SmartRecipes.Mobile.WriteModels;
using SmartRecipes.Mobile.Models;
using System.Collections.Immutable;

namespace SmartRecipes.Mobile.ViewModels
{
    public class NewRecipeAddIngredient : ViewModel
    {
        private readonly MyRecipesHandler commandHandler;

        private readonly FoodstuffRepository repository;

        public NewRecipeAddIngredient(MyRecipesHandler commandHandler, FoodstuffRepository repository)
        {
            this.repository = repository;
            this.commandHandler = commandHandler;
            SearchResult = ImmutableList.Create<FoodstuffCellViewModel>();
        }

        public IEnumerable<FoodstuffCellViewModel> SearchResult { get; private set; }

        public async void Search(string query)
        {
            var searchResult = await repository.Search(query);
            // SearchResult = searchResult.Select(f => new FoodstuffCellViewModel(f, f.BaseAmount, () => Add(f)));
            RaisePropertyChanged(nameof(SearchResult));
        }
    }
}
