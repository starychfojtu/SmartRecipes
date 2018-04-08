using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SmartRecipes.Mobile.Controllers;

namespace SmartRecipes.Mobile
{
    public class MyRecipesViewModel : ViewModel
    {
        readonly MyRecipesController controller;

        public MyRecipesViewModel(MyRecipesController controller)
        {
            this.controller = controller;
        }

        public async Task<IEnumerable<RecipeCellViewModel>> GetRecipes()
        {
            return (await controller.GetAll()).Select(r => RecipeCellViewModel.Create(r, () => { }));
        }
    }
}
