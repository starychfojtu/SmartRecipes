using System.Collections.Generic;
using SmartRecipes.Mobile.ApiDto;
using System.Threading.Tasks;
using SmartRecipes.Mobile.Commands;
using SmartRecipes.Mobile.ReadModels;

namespace SmartRecipes.Mobile.Controllers
{
    public class ShoppingListHandler
    {
        private readonly ApiClient apiClient;

        public ShoppingListHandler(ApiClient apiClient)
        {
            this.apiClient = apiClient;
        }

        public async Task<IEnumerable<Ingredient>> Handle(DecreaseAmount command)
        {
            var request = new AdjustItemInShoppingListRequest(command.Ingredient.Foodstuff.Id, AdjustShoppingListItemAction.DecreaseAmount);
            var response = await apiClient.Post(request);
            return ShoppingListRepository.ToIngredients(response.Items);
        }

        public async Task<IEnumerable<Ingredient>> Handle(IncreaseAmount command)
        {
            return await IncreaseAmount(command.Ingredient.Foodstuff);
        }

        public async Task<IEnumerable<Ingredient>> Handle(AddToShoppingList command)
        {
            return await IncreaseAmount(command.Foodstuff);
        }

        private async Task<IEnumerable<Ingredient>> IncreaseAmount(Foodstuff foodstuff)
        {
            var request = new AdjustItemInShoppingListRequest(foodstuff.Id, AdjustShoppingListItemAction.IncreaseAmount);
            var response = await apiClient.Post(request);
            return ShoppingListRepository.ToIngredients(response.Items);
        }
    }
}
