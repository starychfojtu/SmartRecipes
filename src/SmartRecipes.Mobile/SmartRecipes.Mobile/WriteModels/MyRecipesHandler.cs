using System.Collections.Generic;
using System.Threading.Tasks;
using System.Collections;
using System.Linq;

namespace SmartRecipes.Mobile.Controllers
{
    public class MyRecipesHandler
    {
        private readonly ApiClient apiClient;

        public MyRecipesHandler(ApiClient apiClient)
        {
            this.apiClient = apiClient;
        }
    }
}
