using SmartRecipes.Mobile.ApiDto;
using System.Threading.Tasks;
using System;
using LanguageExt;

namespace SmartRecipes.Mobile.Controllers
{
    public class ShoppingListHandler
    {
        private readonly ApiClient apiClient;

        public ShoppingListHandler(ApiClient apiClient)
        {
            this.apiClient = apiClient;
        }

        public async Task<Ingredient> DecreaseAmount(Ingredient ingredient)
        {
            return await ChangeAmount(ingredient, i => Ingredient.DecreaseAmount(i), IngredientAction.DecreaseAmount);
        }

        public async Task<Ingredient> IncreaseAmount(Ingredient ingredient)
        {
            return await ChangeAmount(ingredient, i => Ingredient.IncreaseAmount(i), IngredientAction.IncreaseAmount);
        }

        private async Task<Ingredient> ChangeAmount(Ingredient ingredient, Func<Ingredient, Option<Ingredient>> operation, IngredientAction action)
        {
            var changed = operation(ingredient).IfNone(() => throw new InvalidOperationException());
            var request = new AdjustIngredientRequest(ingredient.Foodstuff.Id, action);
            var response = await apiClient.Post(request);
            // TODO: create job to update api

            return changed;
        }

        public async Task Handle(AddToShoppingList command)
        {
            //await IncreaseAmount(command.Foodstuff);
        }
    }
}
