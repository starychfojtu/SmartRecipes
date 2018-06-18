using System.Threading.Tasks;
using SmartRecipes.Mobile.Models;
using SmartRecipes.Mobile.Services;
using System.Collections.Generic;
using LanguageExt;

namespace SmartRecipes.Mobile.WriteModels
{
    public static class MyRecipesHandler
    {
        public static Task<Unit> Add(
            Enviroment enviroment,
            IRecipe recipe,
            IEnumerable<IFoodstuffAmount> ingredients)
        {
            return enviroment.Db.AddAsync(recipe)
                .Bind(_ => enviroment.Db.AddAsync(ingredients))
                .Map(_ => Unit.Default);
        }

        public static Task<Unit> Update(Enviroment enviroment, IRecipe recipe, IEnumerable<IIngredientAmount> ingredients)
        {
            return enviroment.Db.UpdateAsync(recipe)
                .Bind(_ => DeleteIngredients(enviroment.Db, recipe))
                .Bind(_ => enviroment.Db.AddAsync(ingredients))
                .Map(_ => Unit.Default);
        }

        private static Task<Unit> DeleteIngredients(Database database, IRecipe recipe)
        {
            var ingredientAmounts = database.GetTableMapping<IngredientAmount>();
            var recipeId = ingredientAmounts.FindColumnWithPropertyName(nameof(IngredientAmount.RecipeId)).Name;
            var deleteCommand = $@"DELETE FROM {ingredientAmounts.TableName} WHERE {recipeId} = ?";

            return database.Execute<int>(deleteCommand, recipe.Id).Map(_ => Unit.Default);
        }
    }
}
