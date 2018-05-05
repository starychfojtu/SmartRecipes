using System;
using SQLite;

namespace SmartRecipes.Mobile.Models
{
    public class FoodstuffAmount : IFoodstuffAmount
    {
        private FoodstuffAmount(Guid id, Guid foodstuffId, IAmount amount)
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
        public IAmount Amount
        {
            get { return new Amount(_Count, _Unit); }
            set
            {
                _Count = value.Count;
                _Unit = value.Unit;
            }
        }

        public IFoodstuffAmount WithAmount(IAmount amount)
        {
            return new FoodstuffAmount(Id, FoodstuffId, amount);
        }

        public IFoodstuffAmount WithRecipe(IRecipe recipe)
        {
            return (new FoodstuffAmount(Id, FoodstuffId, Amount)).Tee(f => f.RecipeId = recipe.Id);
        }

        // Combinators

        public static IFoodstuffAmount Create(Guid id, IFoodstuff foodstuff, IAmount amount)
        {
            return new FoodstuffAmount(id, foodstuff.Id, amount);
        }

        public static IFoodstuffAmount Create(Guid id, IFoodstuff foodstuff)
        {
            return new FoodstuffAmount(id, foodstuff.Id, foodstuff.BaseAmount);
        }

        public static IFoodstuffAmount CreateForRecipe(Guid id, Guid recipeId, Guid foodstuffId, IAmount amount)
        {
            return new FoodstuffAmount(id, foodstuffId, amount)
            {
                RecipeId = recipeId
            };
        }
    }
}
