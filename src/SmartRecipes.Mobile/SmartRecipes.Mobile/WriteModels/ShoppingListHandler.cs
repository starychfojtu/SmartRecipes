using SmartRecipes.Mobile.ApiDto;
using System.Threading.Tasks;
using System;
using LanguageExt;
using SmartRecipes.Mobile.Services;
using SmartRecipes.Mobile.ReadModels.Dto;
using SmartRecipes.Mobile.Models;
using LanguageExt.SomeHelp;
using System.Collections.Generic;
using System.Linq;

namespace SmartRecipes.Mobile.WriteModels
{
    public class ShoppingListHandler
    {
        private readonly ApiClient apiClient;

        private readonly Database database;

        public ShoppingListHandler(ApiClient apiClient, Database database)
        {
            this.apiClient = apiClient;
            this.database = database;
        }

        public async Task<IEnumerable<Ingredient>> DecreaseAmount(IEnumerable<Ingredient> ingredients)
        {
            return await ChangeAmount(ingredients, Amount.Substract, IngredientAction.DecreaseAmount);
        }

        public async Task<IEnumerable<Ingredient>> IncreaseAmount(IEnumerable<Ingredient> ingredients)
        {
            return await ChangeAmount(ingredients, Amount.Add, IngredientAction.IncreaseAmount);
        }

        public async Task<IEnumerable<Ingredient>> Add(IEnumerable<IFoodstuff> foodstuffs)
        {
            // TODO: add proper user id
            var ingredients = foodstuffs.Select(f => new Ingredient(f.ToSome(), FoodstuffAmount.CreateForShoppingList(Guid.NewGuid(), Guid.NewGuid(), f.Id, f.BaseAmount).ToSome()));

            var shoppingListItems = ingredients.Select(i => i.FoodstuffAmount);
            await database.AddAsync(shoppingListItems);

            return await IncreaseAmount(ingredients);
        }

        private async Task<IEnumerable<Ingredient>> ChangeAmount(
            IEnumerable<Ingredient> ingredients,
            Func<IAmount, IAmount, Option<IAmount>> operation,
            IngredientAction action)
        {
            var newIngredients = ingredients.Select(i =>
            {
                var newAmount = operation(i.Amount, i.Foodstuff.AmountStep).IfNone(() => throw new InvalidOperationException());
                return i.WithAmount(newAmount.ToSome());
            });

            foreach (var ingredient in ingredients)
            {
                // TODO: create job to update api
                var request = new AdjustIngredientRequest(ingredient.Foodstuff.Id, action);
                var response = await apiClient.Post(request);
            }

            await database.UpdateAsync(newIngredients.Select(i => i.FoodstuffAmount));
            var test = await database.FoodstuffAmounts.ToListAsync();

            return newIngredients;
        }
    }
}
