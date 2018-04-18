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

        public async Task<ShoppingListItem> DecreaseAmount(ShoppingListItem item)
        {
            return item.WithIngredient(await ChangeAmount(item, Amount.Substract, IngredientAction.DecreaseAmount));
        }

        public async Task<ShoppingListItem> IncreaseAmount(ShoppingListItem item)
        {
            return item.WithIngredient(await ChangeAmount(item, Amount.Add, IngredientAction.IncreaseAmount));
        }

        public async Task<ShoppingListItem> Add(Foodstuff foodstuff)
        {
            return await IncreaseAmount(new ShoppingListItem(foodstuff, Ingredient.Create(foodstuff))); // TODO: notify DB
        }

        private async Task<Ingredient> ChangeAmount(ShoppingListItem item, Func<Amount, Amount, Option<Amount>> operation, IngredientAction action)
        {
            var test = await database.Ingredients.ToListAsync();
            var test2 = await database.Foodstuffs.ToListAsync();

            // Main business action - should be pure
            var newAmount = operation(item.Amount, item.Foodstuff.AmountStep);
            var changedIngredient = item.Ingredient.WithAmount(newAmount.IfNone(() => throw new InvalidOperationException()));

            // Notifying API
            var request = new AdjustIngredientRequest(item.Foodstuff.Id, action);
            var response = await apiClient.Post(request);

            // Notifying DB
            await database.UpdateAsync(changedIngredient);
            // TODO: create job to update api

            // Returning
            return changedIngredient;
        }
    }
}
