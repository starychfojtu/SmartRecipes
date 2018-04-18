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
            public Recipe(Guid id, string name, Uri imageUrl, Guid ownerId, int personCount, string text, IEnumerable<Ingredient> ingredients)
            {
                Id = id;
                Name = name;
                ImageUrl = imageUrl;
                OwnerId = ownerId;
                PersonCount = personCount;
                Text = text;
                Ingredients = ingredients;
            }

            public Guid Id { get; }

            public string Name { get; }

            public Uri ImageUrl { get; }

            public Guid OwnerId { get; }

            public int PersonCount { get; }

            public string Text { get; set; }

            public IEnumerable<Ingredient> Ingredients { get; }

            public class Ingredient
            {
                public Ingredient(Guid id, Guid foodstuffId, Amount amount)
                {
                    Id = id;
                    FoodstuffId = foodstuffId;
                    Amount = amount;
                }

                public Guid Id { get; }

                public Guid FoodstuffId { get; }

                public Amount Amount { get; }
            }
        }
    }
}
