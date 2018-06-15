using System.Linq;
using System.Collections.Generic;
using SmartRecipes.Mobile.Models;
using System.Collections.Immutable;
using System.Threading.Tasks;
using SmartRecipes.Mobile.ReadModels;
using SmartRecipes.Mobile.Services;

namespace SmartRecipes.Mobile.ViewModels
{
    public class FoodstuffSearchViewModel : ViewModel
    {
        private readonly DataAccess dataAccess;

        private IEnumerable<IFoodstuff> searched;

        public FoodstuffSearchViewModel(DataAccess dataAccess)
        {
            searched = ImmutableList.Create<IFoodstuff>();
            Selected = ImmutableList.Create<IFoodstuff>();
            this.dataAccess = dataAccess;
        }

        public IEnumerable<FoodstuffSearchCellViewModel> SearchResult
        {
            get { return searched.Except(Selected).Select(f => new FoodstuffSearchCellViewModel(f, () => Add(f))); }
        }

        public IImmutableList<IFoodstuff> Selected { get; set; }

        public async Task Search(string query)
        {
            searched = await FoodstuffRepository.Search(query)(dataAccess);
            RaisePropertyChanged(nameof(SearchResult));
        }

        private void Add(IFoodstuff foodstuff)
        {
            Selected = Selected.Add(foodstuff);
            RaisePropertyChanged(nameof(SearchResult));
        }
    }
}
