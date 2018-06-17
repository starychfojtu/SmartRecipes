using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SmartRecipes.Mobile.ApiDto;
using SmartRecipes.Mobile.Services;
using SmartRecipes.Mobile.ReadModels.Dto;
using SmartRecipes.Mobile.Models;
using System.Collections.Immutable;
using SmartRecipes.Mobile.Extensions;

namespace SmartRecipes.Mobile.ReadModels
{
    public static class ShoppingListRepository
    {
        public static Monad.Reader<DataAccess, Task<IEnumerable<ShoppingListItem>>> GetItems(IAccount owner)
        {
            return Repository.RetrievalAction(
                client => client.GetShoppingList(),
                GetShoppingListItems(),
                response => response.Items.Select(i => ToShoppingListItem(i, owner)),
                items => items.SelectMany(i => new object[] { i.Foodstuff, i.ItemAmount })
            );
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
            return new ShoppingListItem(foodstuff, ShoppingListItemAmount.Create(i.Id, owner.Id, foodstuff.Id, i.Amount));
        }

        public static Monad.Reader<DataAccess, Task<IEnumerable<ShoppingListRecipeItem>>> GetRecipeItems(IAccount owner)
        {
            return
                from rs in GetRecipesInShoppingList(owner)
                from rs2 in RecipeRepository.GetRecipes(rs.Select(r => r.RecipeId))
                from ds in RecipeRepository.GetDetails(rs2)
                select rs.Join(ds, r => r.RecipeId, d => d.Recipe.Id, (r, d) => new ShoppingListRecipeItem(d, r));
        }

        public static Monad.Reader<DataAccess, Task<ImmutableDictionary<IFoodstuff, IAmount>>> GetRequiredAmounts(IAccount owner)
        {
            var result = ImmutableDictionary.Create<IFoodstuff, IAmount>();
            return GetRecipeItems(owner).Select(rs => rs.Fold(result, (dict, item) =>
            {
                return item.Detail.Ingredients.Fold(dict, (d, i) =>
                {
                    var personCountRatio = item.RecipeInShoppingList.PersonCount / item.Detail.Recipe.PersonCount;
                    var amount = i.Amount.WithCount(i.Amount.Count * personCountRatio); 
                    var newAmount = d.ContainsKey(i.Foodstuff)
                        ? Amount.Add(d[i.Foodstuff], amount).IfNone(amount)
                        : amount;
                    return d.SetItem(i.Foodstuff, newAmount);
                });
            }));
        }

        private static Monad.Reader<DataAccess, Task<IEnumerable<ShoppingListItem>>> GetShoppingListItems()
        {
            return
                from lis in GetShoppingListItemAmounts()
                from fs in GetFoodstuffs(lis.Select(i => i.FoodstuffId))
                select lis.Join(fs, i => i.FoodstuffId, f => f.Id, (i, f) => new ShoppingListItem(f, i));
        }

        private static Monad.Reader<DataAccess, Task<IEnumerable<IShoppingListItemAmount>>> GetShoppingListItemAmounts()
        {
            return da => da.Db.ShoppingListItemAmounts.ToEnumerableAsync<ShoppingListItemAmount, IShoppingListItemAmount>();
        }

        private static Monad.Reader<DataAccess, Task<IEnumerable<IFoodstuff>>> GetFoodstuffs(IEnumerable<Guid> ids)
        {
            return da => da.Db.Foodstuffs.Where(f => ids.Contains(f.Id)).ToEnumerableAsync<Foodstuff, IFoodstuff>();
        }

        private static Monad.Reader<DataAccess, Task<IEnumerable<IRecipeInShoppingList>>> GetRecipesInShoppingList(IAccount owner)
        {
            return da => da.Db.RecipeInShoppingLists
                .Where(r => r.ShoppingListOwnerId == owner.Id)
                .ToEnumerableAsync<RecipeInShoppingList, IRecipeInShoppingList>();
        }
    }
}
