using System.Linq;
using System.Collections.Generic;
using SmartRecipes.Mobile.ReadModels;
using SmartRecipes.Mobile.WriteModels;
using SmartRecipes.Mobile.Models;
using System.Collections.Immutable;
using System.Threading.Tasks;

namespace SmartRecipes.Mobile.ViewModels
{
    public class FoodstuffSearchViewModel : ViewModel
    {
        private readonly ShoppingListHandler commandHandler;

        private readonly FoodstuffRepository repository;

        private IEnumerable<IFoodstuff> searched;

        public FoodstuffSearchViewModel(ShoppingListHandler commandHandler, FoodstuffRepository repository)
        {
            this.repository = repository;
            this.commandHandler = commandHandler;
            searched = ImmutableList.Create<IFoodstuff>();
            Selected = ImmutableList.Create<IFoodstuff>();
        }

        public IEnumerable<FoodstuffSearchCellViewModel> SearchResult
        {
            get { return searched.Except(Selected).Select(f => new FoodstuffSearchCellViewModel(f, () => Add(f))); }
        }

        public IImmutableList<IFoodstuff> Selected { get; set; }

        public async Task Search(string query)
        {
            searched = await repository.Search(query);
            RaisePropertyChanged(nameof(SearchResult));
        }

        private void Add(IFoodstuff foodstuff)
        {
            Selected = Selected.Add(foodstuff);
            RaisePropertyChanged(nameof(SearchResult));
        }
    }
}
