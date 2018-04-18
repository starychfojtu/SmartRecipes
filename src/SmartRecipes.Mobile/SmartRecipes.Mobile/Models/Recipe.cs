using System.Collections.Generic;
using System;
using SQLite;

namespace SmartRecipes.Mobile
{
    public class Recipe
    {
        private Recipe(Guid id, Guid ownerId, string name, Uri imageUrl, int personCount, string text, IEnumerable<Ingredient> ingredients)
        {
            Id = id;
            Name = name;
            ImageUrl = imageUrl;
            OwnerId = ownerId;
            PersonCount = personCount;
            Ingredients = ingredients;
            Text = text;
        }

        public Recipe() { /* for sqllite */ }

        [PrimaryKey]
        public Guid Id { get; set; }

        public Guid OwnerId { get; set; }

        public string Name { get; set; }

        public string _ImageUrl { get; set; }
        [Ignore]
        public Uri ImageUrl
        {
            get { return new Uri(_ImageUrl); }
            set { _ImageUrl = value.AbsoluteUri; }
        }

        public int PersonCount { get; set; }

        [Ignore]
        public IEnumerable<Ingredient> Ingredients { get; } // TODO: serialize as json array field

        public string Text { get; set; }

        // Combinators

        public static Recipe Create(Guid id, Guid ownerId, string name, Uri imageUrl, int personCount, string text, IEnumerable<Ingredient> ingredients)
        {
            return new Recipe(id, ownerId, name, imageUrl, personCount, text, ingredients);
        }
    }
}
