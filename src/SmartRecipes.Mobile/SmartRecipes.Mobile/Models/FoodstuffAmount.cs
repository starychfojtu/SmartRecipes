using System;
using SQLite;

namespace SmartRecipes.Mobile.Models
{
    public class FoodstuffAmount
    {
        private FoodstuffAmount(Guid id, Guid foodstuffId, Amount amount)
        {
            Id = id;
            FoodstuffId = foodstuffId;
            Amount = amount;
        }

        public FoodstuffAmount() { /* for sqllite */ }

        [PrimaryKey]
        public Guid Id { get; set; }

        public Guid FoodstuffId { get; set; }

        public Guid? RecipeId { get; set; }

        public Guid? ShoppingListOwnerId { get; set; }

        public int _Count { get; set; }
        public AmountUnit _Unit { get; set; }
        [Ignore]
        public Amount Amount
        {
            get { return new Amount(_Count, _Unit); }
            set
            {
                _Count = value.Count;
                _Unit = value.Unit;
            }
        }

        public FoodstuffAmount WithAmount(Amount amount)
        {
            return new FoodstuffAmount(Id, FoodstuffId, amount);
        }

        // Combinators

        public static FoodstuffAmount Create(Guid id, Foodstuff foodstuff, Amount amount)
        {
            return new FoodstuffAmount(id, foodstuff.Id, amount);
        }

        public static FoodstuffAmount Create(Guid id, Foodstuff foodstuff)
        {
            return new FoodstuffAmount(id, foodstuff.Id, foodstuff.BaseAmount);
        }

        public static FoodstuffAmount CreateForRecipe(Guid id, Guid recipeId, Guid foodstuffId, Amount amount)
        {
            return new FoodstuffAmount(id, foodstuffId, amount)
            {
                RecipeId = recipeId
            };
        }
    }
}
