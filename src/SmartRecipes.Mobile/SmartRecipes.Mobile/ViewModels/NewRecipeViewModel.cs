using SmartRecipes.Mobile.WriteModels;
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

        public FormDto Recipe { get; set; }

        public async void Submit()
        {
            // TODO: add proper owner id
            await commandHandler.Add(Models.Recipe.Create(
                Guid.NewGuid(),
                Guid.NewGuid(),
                Recipe.Name,
                new Uri(Recipe.ImageUrl),
                Recipe.PersonCount,
                Recipe.Text
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
