using System.Collections.Generic;
using System;
using SQLite;

namespace SmartRecipes.Mobile
{
    public class Recipe
    {
        private Recipe(Guid id, string name, Uri imageUrl, Account owner, int personCount, IEnumerable<Ingredient> ingredients, string text)
        {
            Id = id;
            Name = name;
            ImageUrl = imageUrl;
            Owner = owner;
            PersonCount = personCount;
            Ingredients = ingredients;
            Text = text;
        }

        public Recipe() { /* for sqllite */ }

        [PrimaryKey]
        public Guid Id { get; }

        public string Name { get; }

        public Uri ImageUrl { get; }

        public Account Owner { get; }

        public int PersonCount { get; }

        public IEnumerable<Ingredient> Ingredients { get; } // TODO: remove

        public string Text { get; }

        // Combinators

        public static Recipe Create(Guid id, string name, Uri imageUrl, Account owner, int personCount, IEnumerable<Ingredient> ingredients, string text)
        {
            return new Recipe(id, name, imageUrl, owner, personCount, ingredients, text);
        }
    }
}
