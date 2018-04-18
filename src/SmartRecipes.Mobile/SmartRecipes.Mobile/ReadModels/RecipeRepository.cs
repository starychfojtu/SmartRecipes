using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

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
            var owner = new Account(Guid.Parse("13cb78ee-0aca-4287-9ecb-b87b4e83411b"), "someEmail@gmail.com");
            var imageUrl = "https://www.recipetineats.com/wp-content/uploads/2017/05/Lasagne-recipe-3-main-680x952.jpg";
            return new[]
            {
                Recipe.Create(Guid.Parse("a198fb84-42ca-41f8-bf23-2df76eb59b96"), "Lasagna", new Uri(imageUrl), owner, 1, Enumerable.Empty<Ingredient>() , "Cook me"),
                Recipe.Create(Guid.Parse("110d81a1-a18b-43fb-9435-83ea8a1d4678"), "Lasagna 2", new Uri(imageUrl), owner, 2, Enumerable.Empty<Ingredient>(), "Cook me twice")
            };
        }
    }
}
