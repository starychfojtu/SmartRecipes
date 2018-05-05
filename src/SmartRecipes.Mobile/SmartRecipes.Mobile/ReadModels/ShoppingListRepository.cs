using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SmartRecipes.Mobile.ApiDto;
using LanguageExt;
using SmartRecipes.Mobile.Services;
using SmartRecipes.Mobile.ReadModels.Dto;
using SmartRecipes.Mobile.Models;
using LanguageExt.SomeHelp;

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

        public static Ingredient ToIngredients(ShoppingListResponse.Item i)
        {
            var foodstuff = Foodstuff.Create(
                i.FoodstuffDto.Id,
                i.FoodstuffDto.Name,
                i.FoodstuffDto.ImageUrl,
                i.FoodstuffDto.BaseAmount,
                i.FoodstuffDto.AmountStep
            );
            return new Ingredient(foodstuff.ToSome(), FoodstuffAmount.Create(i.Id, foodstuff, i.Amount).ToSome());
        }
    }
}
