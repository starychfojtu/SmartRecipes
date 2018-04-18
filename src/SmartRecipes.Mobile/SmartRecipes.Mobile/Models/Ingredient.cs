﻿using System;
using SQLite;

namespace SmartRecipes.Mobile
{
    public class Ingredient
    {
        private Ingredient(Guid id, Guid foodstuffId, Amount amount)
        {
            Id = id;
            FoodstuffId = foodstuffId;
            Amount = amount;
        }

        public Ingredient() { /* for sqllite */ }

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

        public Ingredient WithAmount(Amount amount)
        {
            return new Ingredient(Id, FoodstuffId, amount);
        }

        // Combinators

        public static Ingredient Create(Guid id, Foodstuff foodstuff, Amount amount)
        {
            return new Ingredient(id, foodstuff.Id, amount);
        }

        public static Ingredient Create(Guid id, Foodstuff foodstuff)
        {
            return new Ingredient(id, foodstuff.Id, foodstuff.BaseAmount);
        }
    }
}
