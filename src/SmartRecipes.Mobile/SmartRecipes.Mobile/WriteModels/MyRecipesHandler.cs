using System.Threading.Tasks;

namespace SmartRecipes.Mobile.Controllers
{
    public class MyRecipesHandler
    {
        private readonly ApiClient apiClient;

        private readonly Database database;

        public MyRecipesHandler(ApiClient apiClient, Database database)
        {
            this.database = database;
            this.apiClient = apiClient;
        }

        public async Task Add(Recipe recipe)
        {
            await database.AddAsync(recipe.ToEnumerable());
        }
    }
}
