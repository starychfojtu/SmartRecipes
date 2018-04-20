using System;
using System.Collections.Generic;

namespace SmartRecipes.Mobile
{
    public class MyRecipesResponse
    {
        public MyRecipesResponse(IEnumerable<Recipe> recipes)
        {
            Recipes = recipes;
        }

        public IEnumerable<Recipe> Recipes { get; }

        public class Recipe
        {
            public Recipe(Guid id, string name, Uri imageUrl, Guid ownerId, int personCount, string text)
            {
                Id = id;
                Name = name;
                ImageUrl = imageUrl;
                OwnerId = ownerId;
                PersonCount = personCount;
                Text = text;
            }

            public Guid Id { get; }

            public string Name { get; }

            public Uri ImageUrl { get; }

            public Guid OwnerId { get; }

            public int PersonCount { get; }

            public string Text { get; set; }
        }
    }
}
