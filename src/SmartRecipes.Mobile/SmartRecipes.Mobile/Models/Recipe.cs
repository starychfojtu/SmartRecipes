using System.Collections.Generic;
using System;
using SQLite;

namespace SmartRecipes.Mobile
{
    public class Recipe
    {
        private Recipe(Guid id, string name, Uri imageUrl, Guid ownerId, int personCount, IEnumerable<Ingredient> ingredients, string text)
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

        public string Name { get; set; }

        public string _ImageUrl { get; set; }
        [Ignore]
        public Uri ImageUrl
        {
            get { return new Uri(_ImageUrl); }
            set { _ImageUrl = value.AbsolutePath; }
        }

        public Guid OwnerId { get; set; }

        public int PersonCount { get; set; }

        public IEnumerable<Ingredient> Ingredients { get; } // TODO: serialize as json array field

        public string Text { get; set; }

        // Combinators

        public static Recipe Create(Guid id, string name, Uri imageUrl, Account owner, int personCount, IEnumerable<Ingredient> ingredients, string text)
        {
            return new Recipe(id, name, imageUrl, owner.Id, personCount, ingredients, text);
        }
    }
}
