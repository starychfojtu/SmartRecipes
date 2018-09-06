using System.Linq;
using System.Collections.Generic;
using SmartRecipes.Mobile.Models;
using System.Collections.Immutable;
using System.Threading.Tasks;
using SmartRecipes.Mobile.Infrastructure;
using SmartRecipes.Mobile.ReadModels;

namespace SmartRecipes.Mobile.ViewModels
{
    public class FoodstuffSearchViewModel : ViewModel
    {
        private readonly Enviroment enviroment;

        private IEnumerable<IFoodstuff> searched;

        public FoodstuffSearchViewModel(Enviroment enviroment)
        {
            searched = ImmutableList.Create<IFoodstuff>();
            Selected = ImmutableList.Create<IFoodstuff>();
            this.enviroment = enviroment;
        }

        public IEnumerable<FoodstuffSearchCellViewModel> SearchResult
        {
            get { return searched.Except(Selected).Select(f => new FoodstuffSearchCellViewModel(f, () => Add(f))); }
        }

        public IImmutableList<IFoodstuff> Selected { get; set; }

        public async Task Search(string query)
        {
            searched = await FoodstuffRepository.Search(query)(enviroment);
            RaisePropertyChanged(nameof(SearchResult));
        }

        private void Add(IFoodstuff foodstuff)
        {
            Selected = Selected.Add(foodstuff);
            RaisePropertyChanged(nameof(SearchResult));
        }
    }
}
