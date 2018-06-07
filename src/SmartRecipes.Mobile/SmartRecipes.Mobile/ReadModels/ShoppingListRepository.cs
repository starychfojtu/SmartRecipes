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
    public static class ShoppingListRepository
    {
        public static async Task<IEnumerable<ShoppingListItem>> GetItems(ApiClient apiClient, Database database, IAccount owner)
        {
            return await Repository.RetrievalAction(
                apiClient,
                database,
                client => client.GetShoppingList(),
                db => GetShoppingListItems(db),
                response => response.Items.Select(i => ToShoppingListItem(i, owner)),
                items => items.Select(i => (object)i.Foodstuff).Concat(items.Select(i => (object)i.ItemAmount))
            );
        }

        private static async Task<IEnumerable<ShoppingListItem>> GetShoppingListItems(Database database)
        {
            var foodstuffAmounts = await GetShoppingListItemAmounts(database);

            var foostuffIds = foodstuffAmounts.Select(a => a.FoodstuffId);
            var foodstuffs = await GetFoodstuffs(foostuffIds, database);

            return foodstuffAmounts.Join(foodstuffs, i => i.FoodstuffId, f => f.Id, (i, f) => new ShoppingListItem(f.ToSome(), i.ToSome()));
        }

        private static async Task<IEnumerable<IShoppingListItemAmount>> GetShoppingListItemAmounts(Database database)
        {
            return await database.ShoppingListItemAmounts.ToEnumerableAsync();
        }

        private static async Task<IEnumerable<IFoodstuff>> GetFoodstuffs(IEnumerable<Guid> ids, Database database)
        {
            return await database.Foodstuffs.Where(f => ids.Contains(f.Id)).ToEnumerableAsync();
        }

        public static ShoppingListItem ToShoppingListItem(ShoppingListResponse.Item i, IAccount owner)
        {
            var foodstuff = Foodstuff.Create(
                i.FoodstuffDto.Id,
                i.FoodstuffDto.Name,
                i.FoodstuffDto.ImageUrl,
                i.FoodstuffDto.BaseAmount,
                i.FoodstuffDto.AmountStep
            );
            return new ShoppingListItem(foodstuff.ToSome(), ShoppingListItemAmount.Create(i.Id, owner.Id, foodstuff.Id, i.Amount).ToSome());
        }
    }
}
