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
using Monad;

namespace SmartRecipes.Mobile.ReadModels
{
    public static class ShoppingListRepository
    {
        public static Monad.Reader<DataAccess, Task<IEnumerable<ShoppingListItem>>> GetItems(Some<IAccount> owner)
        {
            return Repository.RetrievalAction(
                client => client.GetShoppingList(),
                GetShoppingListItems(),
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

        public static Monad.Reader<DataAccess, Task<IEnumerable<ShoppingListRecipeItem>>> GetRecipeItems(IAccount owner)
        {
            return null;
            //return GetRecipesInShoppingList();
        }

        public static Monad.Reader<DataAccess, Task<IImmutableDictionary<IFoodstuff, IAmount>>> GetRequiredAmounts(Some<IAccount> owner)
        {
            throw new NotImplementedException();
            // TODO: Add method GerDetails(Enumerable of recipes)
            /*var recipes = GetRecipesInShoppingList(owner);
            var details = recipes.Bind(rs => rs.Select(r => RecipeRepository.GetDetail(r.RecipeId)));
            return details.Fold(ImmutableDictionary.Create<IFoodstuff, IAmount>(), (dict, tuple) =>
            {
                return tuple.Item1.Ingredients.Fold(dict, (d, i) =>
                {
                    var (recipeDetail, personCount) = tuple;
                    var amount = i.Amount.WithCount(i.Amount.Count * (recipeDetail.Recipe.PersonCount / personCount)); // TODO: add floats to amount
                    var newAmount = d.ContainsKey(i.Foodstuff) ? Amount.Add(d[i.Foodstuff], amount).IfNone(amount) : amount;
                    return d.SetItem(i.Foodstuff, newAmount);
                });
            });*/
        }

        private static Monad.Reader<DataAccess, Task<IEnumerable<RecipeDetail>>> GetDetails(IEnumerable<IRecipe> recipes)
        {
            return null;
        }

        private static Monad.Reader<DataAccess, Task<IEnumerable<ShoppingListItem>>> GetShoppingListItems()
        {
            return GetShoppingListItemAmounts().SelectMany(
                ia => GetFoodstuffs(ia.Select(i => i.FoodstuffId)),
                (fas, fs) => fas.Join(fs, i => i.FoodstuffId, f => f.Id, (i, f) => new ShoppingListItem(f.ToSome(), i.ToSome()))
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

        private static Monad.Reader<DataAccess, Task<IEnumerable<IRecipeInShoppingList>>> GetRecipesInShoppingList(Some<IAccount> owner)
        {
            return da => da.Db.RecipeInShoppingLists
                .Where(r => r.ShoppingListOwnerId == owner.Value.Id)
                .ToEnumerableAsync<RecipeInShoppingList, IRecipeInShoppingList>();
        }
    }
}
