using System;
using LanguageExt;

namespace SmartRecipes.Mobile.Models
{
    public class IngredientAmount : FoodstuffAmount, IIngredientAmount
    {
        private IngredientAmount(Guid id, Guid recipeId, Guid foodstuffId, IAmount amount) : base(id, foodstuffId, amount)
        {
            RecipeId = recipeId;
        }

        public IngredientAmount() { /* SQLite */ }

        public Guid RecipeId { get; set; }

        public IIngredientAmount WithAmount(IAmount amount)
        {
            return new IngredientAmount(Id, RecipeId, FoodstuffId, amount);
        }

        public static IIngredientAmount Create(Guid id, Guid recipeId, Guid foodstuffId, IAmount amount)
        {
            return new IngredientAmount(id, recipeId, foodstuffId, amount);
        }

        public static IIngredientAmount Create(Some<IRecipe> recipe, Some<IFoodstuff> foodstuff, IAmount amount)
        {
            return new IngredientAmount(Guid.NewGuid(), recipe.Value.Id, foodstuff.Value.Id, amount);
        }
    }
}
