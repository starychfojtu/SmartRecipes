using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartRecipes.Mobile
{
    public class RecipeRepository
    {
        private readonly ApiClient apiClient;

        public RecipeRepository(ApiClient apiClient)
        {
            this.apiClient = apiClient;
        }

        public async Task<IEnumerable<Recipe>> GetAllAsync()
        {
            return new[]
            {
                Recipe.Create("Lasagna", null, null, 1, Enumerable.Empty<Ingredient>() , "Cook me"),
                Recipe.Create("Lasagna 2", null, null, 2, Enumerable.Empty<Ingredient>(), "Cook me twice")
            };
        }
    }
}
