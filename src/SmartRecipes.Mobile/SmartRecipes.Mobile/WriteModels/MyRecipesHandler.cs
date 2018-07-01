using System.Threading.Tasks;
using SmartRecipes.Mobile.Models;
using System.Collections.Generic;
using LanguageExt;
using SmartRecipes.Mobile.Extensions;
using SmartRecipes.Mobile.Infrastructure;
using static LanguageExt.Prelude;

namespace SmartRecipes.Mobile.WriteModels
{
    public static class MyRecipesHandler
    {
        public static Task<Unit> Add(Enviroment enviroment, IRecipe recipe, IEnumerable<IFoodstuffAmount> ingredients)
        {
            return enviroment.Db.AddAsync(recipe.ToEnumerable())
                .Bind(_ => enviroment.Db.AddAsync(ingredients));
        }

        public static Task<Unit> Update(Enviroment enviroment, IRecipe recipe, IEnumerable<IIngredientAmount> ingredients)
        {
            return enviroment.Db.UpdateAsync(recipe)
                .Bind(_ => DeleteIngredients(enviroment.Db, recipe))
                .Bind(_ => enviroment.Db.AddAsync(ingredients));
        }
        
        public static TryAsync<Unit> Delete(Enviroment enviroment, IRecipe recipe)
        {
            return TryAsync(() =>
            {
                var recipeInShoppingList = enviroment.Db.GetTableMapping<RecipeInShoppingList>();
                var recipeInShoppingListRecipeId = recipeInShoppingList.FindColumnWithPropertyName(nameof(RecipeInShoppingList.RecipeId));
                var deleteRecipesInShoppingLists = $"DELETE FROM {recipeInShoppingList.TableName} WHERE {recipeInShoppingListRecipeId.Name} = ?";
                
                var ingredientAmounts = enviroment.Db.GetTableMapping<IngredientAmount>();
                var ingredientAmountRecipeId = recipeInShoppingList.FindColumnWithPropertyName(nameof(IngredientAmount.RecipeId));
                var deleteIngredientAMounts = $"DELETE FROM {ingredientAmounts.TableName} WHERE {ingredientAmountRecipeId.Name} = ?";

                return enviroment.Db.Execute(deleteRecipesInShoppingLists, recipe.Id)
                    .Bind(_ => enviroment.Db.Execute(deleteIngredientAMounts, recipe.Id))
                    .Bind(_ => enviroment.Db.Delete(recipe));
            });
        }

        private static Task<Unit> DeleteIngredients(Database database, IRecipe recipe)
        {
            var ingredientAmounts = database.GetTableMapping<IngredientAmount>();
            var recipeId = ingredientAmounts.FindColumnWithPropertyName(nameof(IngredientAmount.RecipeId)).Name;
            var deleteCommand = $@"DELETE FROM {ingredientAmounts.TableName} WHERE {recipeId} = ?";

            return database.Execute(deleteCommand, recipe.Id);
        }
    }
}
