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

        public async Task<IEnumerable<Ingredient>> GetItems(IAccount owner)
        {
            return await RetrievalAction(
                client => client.GetShoppingList(),
                db => GetIngredients(db),
                response => response.Items.Select(i => ToIngredients(i, owner)),
                ingredients => ingredients.Select(i => (object)i.Foodstuff).Concat(ingredients.Select(i => (object)i.FoodstuffAmount))
            );
        }

        private static async Task<IEnumerable<Ingredient>> GetIngredients(Database database)
        {
            var foodstuffAmounts = await GetShoppingListItemAmounts(database);

            var foostuffIds = foodstuffAmounts.Select(a => a.FoodstuffId);
            var foodstuffs = await GetFoodstuffs(foostuffIds, database);

            return foodstuffAmounts.Join(foodstuffs, i => i.FoodstuffId, f => f.Id, (i, f) => new Ingredient(f.ToSome(), i.ToSome()));
        }

        private static async Task<IEnumerable<IFoodstuffAmount>> GetShoppingListItemAmounts(Database database)
        {
            return await database.FoodstuffAmounts.Where(i => i.ShoppingListOwnerId != null).ToEnumerableAsync();
        }

        private static async Task<IEnumerable<IFoodstuff>> GetFoodstuffs(IEnumerable<Guid> ids, Database database)
        {
            return await database.Foodstuffs.Where(f => ids.Contains(f.Id)).ToEnumerableAsync();
        }

        public static Ingredient ToIngredients(ShoppingListResponse.Item i, IAccount owner)
        {
            var foodstuff = Foodstuff.Create(
                i.FoodstuffDto.Id,
                i.FoodstuffDto.Name,
                i.FoodstuffDto.ImageUrl,
                i.FoodstuffDto.BaseAmount,
                i.FoodstuffDto.AmountStep
            );
            return new Ingredient(foodstuff.ToSome(), FoodstuffAmount.CreateForShoppingList(i.Id, owner, foodstuff.Id, i.Amount).ToSome());
        }
    }
}
