using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SmartRecipes.Mobile.ApiDto;

namespace SmartRecipes.Mobile.ReadModels
{
    public class ShoppingListRepository
    {
        private readonly ApiClient apiClient;

        private readonly Database database;

        public ShoppingListRepository(ApiClient apiClient, Database database)
        {
            this.apiClient = apiClient;
            this.database = database;
        }

        public async Task<IEnumerable<Ingredient>> GetItems()
        {
            var apiResponse = await apiClient.GetShoppingList();
            return await apiResponse.MatchAsync(
                r => ToIngredients(r.Items), // TODO : update DB 
                () => database.Ingredients.ToEnumerableAsync()
            );
        }

        public async Task<IEnumerable<Foodstuff>> Search(string query)
        {
            // TODO: implement
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

        // TODO: move elsewhere
        public static IEnumerable<Ingredient> ToIngredients(IEnumerable<ShoppingListResponse.Item> items)
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
