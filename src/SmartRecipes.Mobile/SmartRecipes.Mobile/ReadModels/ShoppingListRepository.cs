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
        public static Monad.Reader<Enviroment, Task<IEnumerable<ShoppingListItem>>> GetItems(IAccount owner)
        {
            return Repository.RetrievalAction(
                client => client.GetShoppingList(),
                GetShoppingListItems(),
                response => response.Items.Select(i => ToShoppingListItem(i, owner)),
                items => items.SelectMany(i => new object[] { i.Foodstuff, i.ItemAmount })
            );
        }

        public static Monad.Reader<Enviroment, Task<IEnumerable<ShoppingListRecipeItem>>> GetRecipeItems(IAccount owner)
        {
            return
                from recipesInList in GetRecipesInShoppingList(owner)
                from recipes in RecipeRepository.GetRecipes(recipesInList.Select(r => r.RecipeId))
                from details in RecipeRepository.GetDetails(recipes)
                select recipesInList.Join(details, r => r.RecipeId, d => d.Recipe.Id, (r, d) => new ShoppingListRecipeItem(d, r));
        }

        public static Monad.Reader<Enviroment, Task<ImmutableDictionary<IFoodstuff, IAmount>>> GetRequiredAmounts(IAccount owner)
        {
            return GetRecipeItems(owner).Select(rs => rs.Fold(
                ImmutableDictionary.Create<IFoodstuff, IAmount>(),
                (r, item) => r.Merge(GetRequiredAmounts(item), (a1, a2) => Amount.Add(a1, a2).IfNone(a2))
            ));
        }
        
        public static ImmutableDictionary<IFoodstuff, IAmount> GetRequiredAmounts(ShoppingListRecipeItem item)
        {
            var result = ImmutableDictionary.Create<IFoodstuff, IAmount>();
            return item.Detail.Ingredients.Fold(result, (tempResult, i) =>
            {
                var personCountRatio = item.RecipeInShoppingList.PersonCount / item.Detail.Recipe.PersonCount;
                var newAmount = i.Amount.WithCount(i.Amount.Count * personCountRatio); 
                var totalAmount = tempResult.ContainsKey(i.Foodstuff)
                    ? Amount.Add(tempResult[i.Foodstuff], newAmount).IfNone(newAmount)
                    : newAmount;
                return tempResult.SetItem(i.Foodstuff, totalAmount);
            });
        }

        private static Monad.Reader<Enviroment, Task<IEnumerable<ShoppingListItem>>> GetShoppingListItems()
        {
            return
                from itemAmounts in GetShoppingListItemAmounts()
                from foodstuffs in GetFoodstuffs(itemAmounts.Select(i => i.FoodstuffId))
                select itemAmounts.Join(foodstuffs, i => i.FoodstuffId, f => f.Id, (i, f) => new ShoppingListItem(f, i));
        }

        private static Monad.Reader<Enviroment, Task<IEnumerable<IShoppingListItemAmount>>> GetShoppingListItemAmounts()
        {
            return env => env.Db.ShoppingListItemAmounts.ToEnumerableAsync<ShoppingListItemAmount, IShoppingListItemAmount>();
        }

        private static Monad.Reader<Enviroment, Task<IEnumerable<IFoodstuff>>> GetFoodstuffs(IEnumerable<Guid> ids)
        {
            return env => env.Db.Foodstuffs.Where(f => ids.Contains(f.Id)).ToEnumerableAsync<Foodstuff, IFoodstuff>();
        }

        private static Monad.Reader<Enviroment, Task<IEnumerable<IRecipeInShoppingList>>> GetRecipesInShoppingList(IAccount owner)
        {
            return env => env.Db.RecipeInShoppingLists
                .Where(r => r.ShoppingListOwnerId == owner.Id)
                .ToEnumerableAsync<RecipeInShoppingList, IRecipeInShoppingList>();
        }
        
        private static ShoppingListItem ToShoppingListItem(ShoppingListResponse.Item i, IAccount owner)
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
    }
}
