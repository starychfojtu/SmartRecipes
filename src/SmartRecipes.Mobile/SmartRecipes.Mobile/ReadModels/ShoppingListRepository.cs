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
using System.Collections.Immutable;

namespace SmartRecipes.Mobile.ReadModels
{
    public static class ShoppingListRepository
    {
        public static async Task<IEnumerable<ShoppingListItem>> GetItems(ApiClient apiClient, Database database, Some<IAccount> owner)
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

        public static ShoppingListItem ToShoppingListItem(ShoppingListResponse.Item i, Some<IAccount> owner)
        {
            var foodstuff = Foodstuff.Create(
                i.FoodstuffDto.Id,
                i.FoodstuffDto.Name,
                i.FoodstuffDto.ImageUrl,
                i.FoodstuffDto.BaseAmount,
                i.FoodstuffDto.AmountStep
            );
            return new ShoppingListItem(foodstuff.ToSome(), ShoppingListItemAmount.Create(i.Id, owner.Value.Id, foodstuff.Id, i.Amount).ToSome());
        }

        public static async Task<IImmutableDictionary<IFoodstuff, IAmount>> GetRequiredAmounts(ApiClient apiClient, Database database, Some<IAccount> owner)
        {
            var recipes = await GetRecipesInShoppingList(database, owner);
            var details = await recipes.SelectAsync(async r => (await RecipeRepository.GetDetail(apiClient, database, r.RecipeId), r.PersonCount));
            return details.Fold(ImmutableDictionary.Create<IFoodstuff, IAmount>(), (dict, tuple) =>
            {
                return tuple.Item1.Ingredients.Fold(dict, (d, i) =>
                {
                    var (recipeDetail, personCount) = tuple;
                    var amount = i.Amount.WithCount(i.Amount.Count * (recipeDetail.Recipe.PersonCount / personCount)); // TODO: add floats to amount
                    var newAmount = d.ContainsKey(i.Foodstuff) ? Amount.Add(d[i.Foodstuff], amount).IfNone(amount) : amount;
                    return d.SetItem(i.Foodstuff, newAmount);
                });
            });
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

        private static async Task<IEnumerable<IRecipeInShoppingList>> GetRecipesInShoppingList(Database database, Some<IAccount> owner)
        {
            var ownerId = owner.Value.Id;
            return await database.RecipeInShoppingLists.Where(r => r.ShoppingListOwnerId == ownerId).ToEnumerableAsync();
        }
    }
}
