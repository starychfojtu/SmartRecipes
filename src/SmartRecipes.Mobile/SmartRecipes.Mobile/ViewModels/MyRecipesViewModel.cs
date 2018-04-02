using System.Collections.Generic;
using System.Linq;

namespace SmartRecipes.Mobile
{
    public class MyRecipesViewModel : ViewModel
    {
        public MyRecipesViewModel(Store store) : base(store)
        {
        }

        public IEnumerable<RecipeCellViewModel> Recipes
        {
            get { return store.MyRecipes.Select(r => RecipeCellViewModel.Create(r, () => { })); }
        }
    }
}
