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
using System.Runtime.CompilerServices;
using Monad;

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
            return GetRecipesInShoppingList(owner).SelectMany(
                rs => RecipeRepository.GetRecipes(rs.Select(r => r.RecipeId)).Bind(rs2 => RecipeRepository.GetDetails(rs2)),
                (rs, ds) => rs.Join(ds, r => r.RecipeId, d => d.Recipe.Id, (r, d) => new ShoppingListRecipeItem(d, r))
            );
        }

        public static Monad.Reader<DataAccess, Task<ImmutableDictionary<IFoodstuff, IAmount>>> GetRequiredAmounts(IAccount owner)
        {
            return GetRecipeItems(owner).Select(t => t.Map(rs => rs
                .Fold(ImmutableDictionary.Create<IFoodstuff, IAmount>(), (dict, item) =>
                {
                    return item.Detail.Ingredients.Fold(dict, (d, i) =>
                    {
                        var personCountRatio = item.Detail.Recipe.PersonCount / item.RecipeInShoppingList.PersonCount;
                        var amount = i.Amount.WithCount(i.Amount.Count * personCountRatio); 
                        var newAmount = d.ContainsKey(i.Foodstuff)
                            ? Amount.Add(d[i.Foodstuff], amount).IfNone(amount)
                            : amount;
                        return d.SetItem(i.Foodstuff, newAmount);
                    });
                })
            ));
        }

        private static Monad.Reader<DataAccess, Task<IEnumerable<ShoppingListItem>>> GetShoppingListItems()
        {
            return GetShoppingListItemAmounts().SelectMany(
                ia => GetFoodstuffs(ia.Select(i => i.FoodstuffId)),
                (fas, fs) => fas.Join(fs, i => i.FoodstuffId, f => f.Id, (i, f) => new ShoppingListItem(f, i))
            );
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
