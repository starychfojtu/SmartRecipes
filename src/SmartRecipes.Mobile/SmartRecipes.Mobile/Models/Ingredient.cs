using System;
using SQLite;

namespace SmartRecipes.Mobile
{
    public class Ingredient
    {
        private Ingredient(Guid foodstuffId, Amount amount)
        {
            FoodstuffId = foodstuffId;
            Amount = amount;
        }

        public Ingredient() { /* for sqllite */ }

        [PrimaryKey]
        public Guid Id { get; }

        public Guid FoodstuffId { get; }

        public Guid? RecipeId { get; }

        public Guid? ShoppingListOwnerId { get; }

        public Amount Amount { get; }

        public Ingredient WithAmount(Amount amount)
        {
            return new Ingredient(FoodstuffId, amount);
        }

        // Combinators

        public static Ingredient Create(Foodstuff foodstuff, Amount amount)
        {
            return new Ingredient(foodstuff.Id, amount);
        }

        public static Ingredient Create(Foodstuff foodstuff)
        {
            return new Ingredient(foodstuff.Id, foodstuff.BaseAmount);
        }
    }
}
