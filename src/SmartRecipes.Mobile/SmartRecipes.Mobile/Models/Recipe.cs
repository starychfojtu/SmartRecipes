using System.Collections.Generic;
using System;

namespace SmartRecipes.Mobile
{
    public class Recipe
    {
        private Recipe(string name, Uri imageUrl, Account owner, int personCount, IEnumerable<Ingredient> ingredients, string text)
        {
            Name = name;
            ImageUrl = imageUrl;
            Owner = owner;
            PersonCount = personCount;
            Ingredients = ingredients;
            Text = text;
        }

        public string Name { get; }

        public Uri ImageUrl { get; }

        public Account Owner { get; }

        public int PersonCount { get; }

        public IEnumerable<Ingredient> Ingredients { get; }

        public string Text { get; }

        // Combinators

        public static Recipe Create(string name, Uri imageUrl, Account owner, int personCount, IEnumerable<Ingredient> ingredients, string text)
        {
            return new Recipe(name, imageUrl, owner, personCount, ingredients, text);
        }
    }
}
