using System.Collections.Generic;
using System.Threading.Tasks;
using System.Collections;
using System.Linq;

namespace SmartRecipes.Mobile.Controllers
{
    public class MyRecipesController
    {
        private readonly ApiClient apiClient;

        public MyRecipesController(ApiClient apiClient)
        {
            this.apiClient = apiClient;
        }

        public async Task<IEnumerable<Recipe>> GetAll()
        {
            return new[]
            {
                Recipe.Create("Lasagna", null, null, 1, Enumerable.Empty<Ingredient>() , "Cook me"),
                Recipe.Create("Lasagna 2", null, null, 2, Enumerable.Empty<Ingredient>(), "Cook me twice")
            };
        }
    }
}
