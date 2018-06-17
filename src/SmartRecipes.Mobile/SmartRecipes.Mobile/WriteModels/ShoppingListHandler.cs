using SmartRecipes.Mobile.ApiDto;
using System.Threading.Tasks;
using System;
using LanguageExt;
using SmartRecipes.Mobile.Services;
using SmartRecipes.Mobile.Models;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Immutable;
using SmartRecipes.Mobile.Extensions;
using SmartRecipes.Mobile.ReadModels;
using SmartRecipes.Mobile.ReadModels.Dto;
using static LanguageExt.Prelude;

namespace SmartRecipes.Mobile.WriteModels
{
    public static class ShoppingListHandler
    {
        public static IShoppingListItemAmount Increase(IShoppingListItemAmount foodstuffAmount, IFoodstuff foodstuff)
        {
            return ChangeAmount((a1, a2) => Amount.Add(a1, a2), foodstuffAmount, foodstuff);
        }

        public static IShoppingListItemAmount Decrease(IShoppingListItemAmount foodstuffAmount, IFoodstuff foodstuff)
        {
            return ChangeAmount((a1, a2) => Amount.Substract(a1, a2), foodstuffAmount, foodstuff);
        }

        private static IShoppingListItemAmount ChangeAmount(Func<IAmount, IAmount, Option<IAmount>> action, IShoppingListItemAmount foodstuffAmount, IFoodstuff foodstuff)
        {
            if (foodstuffAmount.FoodstuffId != foodstuff.Id)
            {
                throw new ArgumentException();
            }

            var newAmount = action(foodstuffAmount.Amount, foodstuff.AmountStep).IfNone(() => throw new ArgumentException());
            return foodstuffAmount.WithAmount(newAmount);
        }

        public static TryAsync<Unit> AddToShoppingList(
            DataAccess dataAccess,
            IRecipe recipe,
            IAccount owner,
            int personCount)
        {
            return TryAsync(async () =>
            {
                var ingredients = await RecipeRepository.GetIngredients(recipe)(dataAccess);
                var recipeFoodstuffs = ingredients.Select(i => i.Foodstuff);

                var shoppingListItems = await ShoppingListRepository.GetItems(owner)(dataAccess);
                var alreadyAddedFoodstuffs = shoppingListItems.Select(i => i.Foodstuff);

                var notAddedFoodstuffs = recipeFoodstuffs.Except(alreadyAddedFoodstuffs);
                var itemAmounts = notAddedFoodstuffs.Select(f =>
                    ShoppingListItemAmount.Create(owner, f, Amount.Zero(f.BaseAmount.Unit)));

                await dataAccess.Db.AddAsync(RecipeInShoppingList.Create(recipe, owner, personCount).ToEnumerable());
                await dataAccess.Db.AddAsync(itemAmounts);
                
                return Unit.Default;
            });
        }

        public static TryAsync<Unit> Cook(DataAccess dataAccess, ShoppingListRecipeItem recipeItem)
        {
            return TryAsync<ShoppingListRecipeItem>(() => throw new InvalidOperationException("Not enought ingredients in shopping list."))
                .Bind(i => RemoveFromShoppingList(dataAccess, i.RecipeInShoppingList));
        }

        public static TryAsync<Unit> RemoveFromShoppingList(DataAccess dataAccess, IRecipeInShoppingList recipe)
        {
            return TryAsync(() => dataAccess.Db.Delete(recipe).Map(t => Unit.Default));
        }

        public static async Task<IEnumerable<ShoppingListItem>> AddToShoppingList(DataAccess dataAccess, IAccount owner, IEnumerable<IFoodstuff> foodstuffs)
        {
            var shoppingListItems = await ShoppingListRepository.GetItems(owner)(dataAccess);
            var alreadyAddedFoodstuffs = shoppingListItems.Select(i => i.Foodstuff);
            var newFoodstuffs = foodstuffs.Except(alreadyAddedFoodstuffs).ToImmutableDictionary(f => f.Id, f => f);
            var newItemAmounts = newFoodstuffs.Values.Select(f => ShoppingListItemAmount.Create(owner, f, f.BaseAmount)).ToImmutableList();

            await dataAccess.Db.AddAsync(newItemAmounts);
            await Update(dataAccess, newItemAmounts);

            return newItemAmounts.Select(fa => new ShoppingListItem(newFoodstuffs[fa.FoodstuffId], fa));
        }

        public static async Task Update(DataAccess dataAccess, IImmutableList<IShoppingListItemAmount> itemAmounts)
        {
            foreach (var itemAmount in itemAmounts)
            {
                // TODO: create job to update api when this fails
                var request = new ChangeFoodstuffAmountRequest(itemAmount.FoodstuffId, itemAmount.Amount);
                var response = await dataAccess.Api.Post(request);
            }

            await dataAccess.Db.UpdateAsync(itemAmounts);
        }
    }
}
