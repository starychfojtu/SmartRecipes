using System;
using LanguageExt;
using SQLite;

namespace SmartRecipes.Mobile.Models
{
    public class Recipe : IRecipe
    {
        private const string DefaultImageUrl = "https://thumbs.dreamstime.com/z/empty-dish-14513513.jpg";

        private Recipe(Guid id, Guid ownerId, Some<string> name, Some<Uri> imageUrl, int personCount, Option<string> text)
        {
            Id = id;
            Name = name;
            ImageUrl = imageUrl;
            OwnerId = ownerId;
            PersonCount = personCount;
            Text = text.IfNone(() => null);
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

        public static IRecipe Create(Some<IAccount> owner, Some<string> name, Option<Uri> imageUrl, int personCount, string text)
        {
            return new Recipe(Guid.NewGuid(), owner.Value.Id, name, imageUrl.IfNone(() => new Uri(DefaultImageUrl)), personCount, text);
        }

        public static Recipe Create(Guid id, Guid ownerId, Some<string> name, Option<Uri> imageUrl, int personCount, string text)
        {
            return new Recipe(id, ownerId, name, imageUrl.IfNone(() => new Uri(DefaultImageUrl)), personCount, text);
        }
    }
}
