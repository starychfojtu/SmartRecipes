using System;
using SQLite;

namespace SmartRecipes.Mobile.Models
{
    public class Recipe : IRecipe
    {
        private Recipe(Guid id, Guid ownerId, string name, Uri imageUrl, int personCount, string text)
        {
            Id = id;
            Name = name;
            ImageUrl = imageUrl;
            OwnerId = ownerId;
            PersonCount = personCount;
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

        public string Text { get; set; }

        // Combinators

        // TODO: should be IRecipe
        public static Recipe Create(Guid id, IAccount owner, string name, Uri imageUrl, int personCount, string text)
        {
            return new Recipe(id, owner.Id, name, imageUrl, personCount, text);
        }

        public static Recipe Create(Guid id, Guid ownerId, string name, Uri imageUrl, int personCount, string text)
        {
            return new Recipe(id, ownerId, name, imageUrl, personCount, text);
        }
    }
}
