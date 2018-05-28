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
using System.Collections.Immutable;
using SmartRecipes.Mobile.ReadModels;

namespace SmartRecipes.Mobile.WriteModels
{
    public class ShoppingListHandler
    {
        private readonly ApiClient apiClient;

        private readonly Database database;

        private readonly UserHandler userHandler;

        private readonly ShoppingListRepository repository;

        public ShoppingListHandler(UserHandler userHandler, ApiClient apiClient, Database database, ShoppingListRepository repository)
        {
            this.repository = repository;
            this.userHandler = userHandler;
            this.apiClient = apiClient;
            this.database = database;
        }

        public static IEnumerable<Ingredient> ChangeAmount(IEnumerable<Ingredient> ingredients, Func<IAmount, IAmount, Option<IAmount>> operation)
        {
            return ingredients.Select(i =>
            {
                var newAmount = operation(i.Amount, i.Foodstuff.AmountStep).IfNone(() => throw new InvalidOperationException());
                return i.WithAmount(newAmount.ToSome());
            });
        }

        public async Task<IEnumerable<Ingredient>> DecreaseAmount(IEnumerable<Ingredient> ingredients)
        {
            var newIngredients = ChangeAmount(ingredients, (a1, a2) => Amount.Substract(a1, a2));
            return await newIngredients.TeeAsync(ins => Update(ins.Select(i => i.FoodstuffAmount).ToImmutableList()));
        }

        public async Task<IEnumerable<Ingredient>> IncreaseAmount(IEnumerable<Ingredient> ingredients)
        {
            var newIngredients = ChangeAmount(ingredients, (a1, a2) => Amount.Add(a1, a2));
            return await newIngredients.TeeAsync(ins => Update(ins.Select(i => i.FoodstuffAmount).ToImmutableList()));
        }

        public async Task<IEnumerable<Ingredient>> Add(IEnumerable<IFoodstuff> foodstuffs)
        {
            var shoppingListItems = await repository.GetItems();
            var alreadyAddedFoodstuffs = shoppingListItems.Select(i => i.Foodstuff);
            var newFoodstuffs = foodstuffs.Except(alreadyAddedFoodstuffs).ToImmutableDictionary(f => f.Id, f => f);
            var newFoodstuffAmounts = newFoodstuffs.Values.Select(
                f => FoodstuffAmount.CreateForShoppingList(Guid.NewGuid(), userHandler.CurrentAccount.Id, f.Id, f.BaseAmount)
            );

            await database.AddAsync(newFoodstuffAmounts);
            await Update(newFoodstuffAmounts);

            return newFoodstuffAmounts.Select(fa => new Ingredient(newFoodstuffs[fa.FoodstuffId].ToSome(), fa.ToSome()));
        }

        private async Task Update(IEnumerable<IFoodstuffAmount> foodstuffAmounts)
        {
            foreach (var foodstuffAmount in foodstuffAmounts)
            {
                // TODO: create job to update api when this fails
                var request = new ChangeFoodstuffAmountRequest(foodstuffAmount.FoodstuffId, foodstuffAmount.Amount);
                var response = await apiClient.Post(request);
            }

            await database.UpdateAsync(foodstuffAmounts);
        }
    }
}
