﻿using SmartRecipes.Mobile.ApiDto;
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
        public static IShoppingListItemAmount Increase(ShoppingListItem item)
        {
            return ChangeAmount((a1, a2) => Amount.Add(a1, a2), item);
        }

        public static IShoppingListItemAmount Decrease(ShoppingListItem item)
        {
            return ChangeAmount((a1, a2) => Amount.Substract(a1, a2), item);
        }

        private static IShoppingListItemAmount ChangeAmount(Func<IAmount, IAmount, Option<IAmount>> action, ShoppingListItem item)
        {
            var newAmount = action(item.ItemAmount.Amount, item.Foodstuff.AmountStep).IfNone(() => throw new ArgumentException());
            return item.ItemAmount.WithAmount(newAmount);
        }

        public static TryAsync<Unit> AddToShoppingList(Enviroment enviroment, IRecipe recipe, IAccount owner, int personCount)
        {
            return TryAsync(async () => await ShoppingListRepository
                .GetRecipeItems(owner)
                .Select(items => items.Find(i => i.Detail.Recipe.Equals(recipe)))(enviroment)
                .MatchAsync(
                    i => AddPersonCount(enviroment, i.RecipeInShoppingList, personCount),
                    () => CreateRecipeInShoppingList(enviroment, recipe, owner, personCount)
                )
            );
        }

        public static TryAsync<Unit> Cook(Enviroment enviroment, ShoppingListRecipeItem recipeItem)
        {
            return TryAsync<ShoppingListRecipeItem>(() =>
                {
                    throw new InvalidOperationException("Not enought ingredients in shopping list.");
                })
                .Bind(i => RemoveFromShoppingList(enviroment, i.RecipeInShoppingList));
        }

        public static TryAsync<Unit> RemoveFromShoppingList(Enviroment enviroment, IRecipeInShoppingList recipe)
        {
            return TryAsync(() => enviroment.Db.Delete(recipe).Map(t => Unit.Default));
        }

        public static async Task<IEnumerable<ShoppingListItem>> AddToShoppingList(Enviroment enviroment, IAccount owner, IEnumerable<IFoodstuff> foodstuffs)
        {
            var shoppingListItems = await ShoppingListRepository.GetItems(owner)(enviroment);
            var alreadyAddedFoodstuffs = shoppingListItems.Select(i => i.Foodstuff);
            var newFoodstuffs = foodstuffs.Except(alreadyAddedFoodstuffs).ToImmutableDictionary(f => f.Id, f => f);
            var newItemAmounts = newFoodstuffs.Values.Select(f => ShoppingListItemAmount.Create(owner, f, f.BaseAmount)).ToImmutableList();

            await enviroment.Db.AddAsync(newItemAmounts);
            await Update(enviroment, newItemAmounts);

            return newItemAmounts.Select(fa => new ShoppingListItem(newFoodstuffs[fa.FoodstuffId], fa));
        }

        public static async Task Update(Enviroment enviroment, IImmutableList<IShoppingListItemAmount> itemAmounts)
        {
            foreach (var itemAmount in itemAmounts)
            {
                var request = new ChangeFoodstuffAmountRequest(itemAmount.FoodstuffId, itemAmount.Amount);
                var response = await enviroment.Api.Post(request);
            }

            await enviroment.Db.UpdateAsync(itemAmounts);
        }
        
        private static async Task<Unit> CreateRecipeInShoppingList(Enviroment enviroment, IRecipe recipe, IAccount owner, int personCount)
        {
            var newItemAmounts = await GetRecipeComplementOfShoppingList(recipe, owner)(enviroment);

            await enviroment.Db.AddAsync(RecipeInShoppingList.Create(recipe, owner, personCount));
            await enviroment.Db.AddAsync(newItemAmounts);

            return Unit.Default;
        }

        private static Task<Unit> AddPersonCount(Enviroment enviroment, IRecipeInShoppingList recipe, int personCount)
        {
            return enviroment.Db.UpdateAsync(recipe.AddPersons(personCount)).Map(_ => Unit.Default);
        }

        private static Monad.Reader<Enviroment, Task<IEnumerable<IShoppingListItemAmount>>> GetRecipeComplementOfShoppingList(IRecipe recipe, IAccount owner)
        {
            return
                from ingredients in RecipeRepository.GetIngredients(recipe)
                from items in ShoppingListRepository.GetItems(owner)
                let foodstuffs = ingredients.Select(i => i.Foodstuff)
                let addedFoodstuffs = items.Select(i => i.Foodstuff)
                select foodstuffs.Except(addedFoodstuffs).Select(f => ShoppingListItemAmount.Create(owner, f, Amount.Zero(f.BaseAmount.Unit)));
        }
    }
}
