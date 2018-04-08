using System;
using System.Collections.Generic;
using System.Linq;
using LanguageExt;
using System.Reactive.Linq;
using SmartRecipes.Mobile.ApiDto;
using System.Threading.Tasks;

namespace SmartRecipes.Mobile.Controllers
{
    public class ShoppingListController
    {
        private readonly ApiClient apiClient;

        public ShoppingListController(ApiClient apiClient)
        {
            this.apiClient = apiClient;
        }

        public async Task<IEnumerable<Ingredient>> DecreaseAmount(Ingredient item)
        {
            var request = new AdjustItemInShoppingListRequest(item.Foodstuff.Id, AdjustShoppingListItemAction.DecreaseAmount);
            var response = await apiClient.Post(request);
            return ToIngredients(response.Items);
        }

        public async Task<IEnumerable<Ingredient>> IncreaseAmount(Ingredient item)
        {
            return await IncreaseAmount(item.Foodstuff);
        }

        public async Task<IEnumerable<Ingredient>> Add(Foodstuff foodstuff)
        {
            return await IncreaseAmount(foodstuff);
        }

        public async Task<IEnumerable<Foodstuff>> Search(string query)
        {
            // TODO: implement with API
            var foodstuff = new[]
            {
                new Foodstuff(
                    Guid.NewGuid(),
                    "Carrot",
                    new Uri("https://www.znaturalfoods.com/698-thickbox_default/carrot-powder-organic.jpg"),
                    new Amount(1, AmountUnit.Piece),
                    new Amount(1, AmountUnit.Piece)
                ),
                new Foodstuff(
                    Guid.NewGuid(),
                    "Bacon",
                    new Uri("https://upload.wikimedia.org/wikipedia/commons/3/31/Made20bacon.png"),
                    new Amount(100, AmountUnit.Gram),
                    new Amount(50, AmountUnit.Gram)
                )
            };
            var shoppingListItem = await GetItems();
            return foodstuff.Except(shoppingListItem.Select(i => i.Foodstuff));
        }

        public async Task<IEnumerable<Ingredient>> GetItems()
        {
            var response = await apiClient.GetShoppingList();
            return ToIngredients(response.Items);
        }

        private async Task<IEnumerable<Ingredient>> IncreaseAmount(Foodstuff foodstuff)
        {
            var request = new AdjustItemInShoppingListRequest(foodstuff.Id, AdjustShoppingListItemAction.IncreaseAmount);
            var response = await apiClient.Post(request);
            return ToIngredients(response.Items);
        }

        // TODO move to another layer
        private static IEnumerable<Ingredient> ToIngredients(IEnumerable<ShoppingListResponse.Item> items)
        {
            return items.Select(i => Ingredient.Create(
                new Foodstuff(
                    i.FoodstuffDto.Id,
                    i.FoodstuffDto.Name,
                    i.FoodstuffDto.ImageUrl,
                    i.FoodstuffDto.BaseAmount,
                    i.FoodstuffDto.AmountStep
                ),
                i.Amount
            ));
        }
    }
}
