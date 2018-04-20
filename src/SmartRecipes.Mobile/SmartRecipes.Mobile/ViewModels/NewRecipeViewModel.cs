using SmartRecipes.Mobile.Controllers;
using System;

namespace SmartRecipes.Mobile
{
    public class NewRecipeViewModel
    {
        private readonly MyRecipesHandler commandHandler;

        public NewRecipeViewModel(MyRecipesHandler commandHandler)
        {
            this.commandHandler = commandHandler;
        }

        public FormDto RecipeDto { get; set; }

        public async void Submit()
        {
            // TODO: add proper owner id
            await commandHandler.Add(Recipe.Create(
                Guid.NewGuid(),
                Guid.NewGuid(),
                RecipeDto.Name,
                new Uri(RecipeDto.ImageUrl),
                RecipeDto.PersonCount,
                RecipeDto.Text
            ));
        }

        public class FormDto
        {
            public string Name { get; set; }

            public string ImageUrl { get; set; }

            public int PersonCount { get; set; }

            public string Text { get; set; }
        }
    }
}
