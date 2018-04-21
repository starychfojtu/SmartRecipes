using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SmartRecipes.Mobile.ApiDto;
using LanguageExt;
using SmartRecipes.Mobile.Services;
using SmartRecipes.Mobile.ReadModels.Dto;
using SmartRecipes.Mobile.Models;

namespace SmartRecipes.Mobile.ReadModels
{
    public class ShoppingListRepository : Repository
    {
        public ShoppingListRepository(ApiClient apiClient, Database database) : base(apiClient, database)
        {
        }

        public async Task<IEnumerable<Ingredient>> GetItems()
        {
            return await RetrievalAction(
                client => client.GetShoppingList(),
                db => GetIngredients(db),
                response => response.Items.Select(i => ToIngredients(i)),
                ingredients => ingredients.Select(i => (object)i.Foodstuff).Concat(ingredients.Select(i => (object)i.FoodstuffAmount))
            );
        }

        private static async Task<IEnumerable<Ingredient>> GetIngredients(Database database)
        {
            var ingredients = await database.FoodstuffAmounts.Where(i => i.ShoppingListOwnerId != null).ToEnumerableAsync();
            var foostuffIds = ingredients.Select(i => i.FoodstuffId);
            var foodstuffs = await database.Foodstuffs.Where(f => foostuffIds.Contains(f.Id)).ToEnumerableAsync();
            return ingredients.Join(foodstuffs, i => i.FoodstuffId, f => f.Id, (i, f) => new Ingredient(f, i));
        }

        public async Task<IEnumerable<Foodstuff>> Search(string query)
        {
            // TODO: implement
            var foodstuff = new[]
            {
                Foodstuff.Create(
                    Guid.Parse("cb3d0f54-c99d-43f1-ade4-e316b0e6543d"),
                    "Carrot",
                    new Uri("https://www.znaturalfoods.com/698-thickbox_default/carrot-powder-organic.jpg"),
                    new Amount(1, AmountUnit.Piece),
                    new Amount(1, AmountUnit.Piece)
                ),
                Foodstuff.Create(
                    Guid.Parse("e04ef558-1305-408e-9d26-1f04b7e3f785"),
                    "Bacon",
                    new Uri("https://upload.wikimedia.org/wikipedia/commons/3/31/Made20bacon.png"),
                    new Amount(100, AmountUnit.Gram),
                    new Amount(50, AmountUnit.Gram)
                )
            };
            var shoppingListItem = await GetItems();
            return foodstuff.Except(shoppingListItem.Select(i => i.Foodstuff));
        }

        public static Ingredient ToIngredients(ShoppingListResponse.Item i)
        {
            var foodstuff = Foodstuff.Create(
                i.FoodstuffDto.Id,
                i.FoodstuffDto.Name,
                i.FoodstuffDto.ImageUrl,
                i.FoodstuffDto.BaseAmount,
                i.FoodstuffDto.AmountStep
            );
            return new Ingredient(foodstuff, FoodstuffAmount.Create(i.Id, foodstuff, i.Amount));
        }
    }
}
