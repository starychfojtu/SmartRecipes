using SmartRecipes.Mobile.ApiDto;
using System.Threading.Tasks;
using System;
using LanguageExt;

namespace SmartRecipes.Mobile.Controllers
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

        public async Task<ShoppingListItem> DecreaseAmount(Foodstuff foodstuff)
        {
            return new ShoppingListItem(foodstuff, await ChangeAmount(foodstuff, Amount.Substract, IngredientAction.DecreaseAmount));
        }

        public async Task<ShoppingListItem> IncreaseAmount(Foodstuff foodstuff)
        {
            return new ShoppingListItem(foodstuff, await ChangeAmount(foodstuff, Amount.Add, IngredientAction.IncreaseAmount));
        }

        public async Task<ShoppingListItem> Add(Foodstuff foodstuff)
        {
            return await IncreaseAmount(foodstuff);
        }

        private async Task<Amount> ChangeAmount(Foodstuff foodstuff, Func<Amount, Amount, Option<Amount>> operation, IngredientAction action)
        {
            var ingredientOptional = await database.Ingredients.Where(i => i.FoodstuffId == foodstuff.Id).FirstOptionAsync();
            var ingredient = ingredientOptional.IfNone(() => Ingredient.Create(foodstuff)); // TODO: save to db
            var newAmount = operation(ingredient.Amount, foodstuff.AmountStep);
            var changedIngredient = ingredient.WithAmount(newAmount.IfNone(() => throw new InvalidOperationException()));
            var request = new AdjustIngredientRequest(foodstuff.Id, action);
            var response = await apiClient.Post(request);

            await database.UpdateAsync(changedIngredient);
            // TODO: create job to update api

            return changedIngredient.Amount;
        }
    }
}
