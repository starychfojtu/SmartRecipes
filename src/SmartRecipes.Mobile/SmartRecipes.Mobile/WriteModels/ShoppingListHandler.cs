using SmartRecipes.Mobile.ApiDto;
using System.Threading.Tasks;
using System;
using LanguageExt;
using SmartRecipes.Mobile.Services;
using SmartRecipes.Mobile.ReadModels.Dto;
using SmartRecipes.Mobile.Models;
using LanguageExt.SomeHelp;
using System.Collections.Generic;

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

        public async Task<Ingredient> DecreaseAmount(Ingredient ingredient)
        {
            return await ChangeAmount(ingredient, Amount.Substract, IngredientAction.DecreaseAmount);
        }

        public async Task<Ingredient> IncreaseAmount(Ingredient ingredient)
        {
            return await ChangeAmount(ingredient, Amount.Add, IngredientAction.IncreaseAmount);
        }

        public async Task<IEnumerable<Ingredient>> Add(IEnumerable<IFoodstuff> foodstuffs)
        {
            // TODO: do this as batch
            var ingredients = new List<Ingredient>();
            foreach (var foodstuff in foodstuffs)
            {
                ingredients.Add(await IncreaseAmount(new Ingredient(
                    foodstuff.ToSome(),
                    FoodstuffAmount.Create(Guid.NewGuid(), foodstuff).ToSome()
                ))); // TODO: notify DB
            }
            return ingredients;
        }

        private async Task<Ingredient> ChangeAmount(
            Ingredient ingredient,
            Func<IAmount, IAmount, Option<IAmount>> operation,
            IngredientAction action)
        {
            // Main business action
            var newAmount = operation(ingredient.Amount, ingredient.Foodstuff.AmountStep).IfNone(() => throw new InvalidOperationException());
            var changedIngredient = ingredient.WithAmount(newAmount.ToSome());

            // Notifying API
            var request = new AdjustIngredientRequest(ingredient.Foodstuff.Id, action);
            var response = await apiClient.Post(request);

            // Notifying DB
            await database.UpdateAsync(changedIngredient.FoodstuffAmount);
            // TODO: create job to update api

            return changedIngredient;
        }
    }
}
