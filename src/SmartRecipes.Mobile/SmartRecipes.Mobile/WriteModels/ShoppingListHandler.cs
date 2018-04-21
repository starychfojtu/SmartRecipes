﻿using SmartRecipes.Mobile.ApiDto;
using System.Threading.Tasks;
using System;
using LanguageExt;
using SmartRecipes.Mobile.Services;
using SmartRecipes.Mobile.ReadModels.Dto;
using SmartRecipes.Mobile.Models;

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

        public async Task<Ingredient> Add(Foodstuff foodstuff)
        {
            return await IncreaseAmount(new Ingredient(foodstuff, FoodstuffAmount.Create(Guid.NewGuid(), foodstuff))); // TODO: notify DB
        }

        private async Task<Ingredient> ChangeAmount(Ingredient ingredient, Func<Amount, Amount, Option<Amount>> operation, IngredientAction action)
        {
            // Main business action
            var newAmount = operation(ingredient.Amount, ingredient.Foodstuff.AmountStep);
            var changedIngredient = ingredient.WithAmount(newAmount.IfNone(() => throw new InvalidOperationException()));

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
