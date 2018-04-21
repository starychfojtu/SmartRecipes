using SmartRecipes.Mobile.WriteModels;
using System;
using SmartRecipes.Mobile.ViewModels;
using System.Threading.Tasks;

namespace SmartRecipes.Mobile
{
    public class NewRecipeViewModel : ViewModel
    {
        private readonly MyRecipesHandler commandHandler;

        private const string DefaultImageUrl = "https://thumbs.dreamstime.com/z/empty-dish-14513513.jpg";

        public NewRecipeViewModel(MyRecipesHandler commandHandler)
        {
            this.commandHandler = commandHandler;
            Recipe = new FormDto();
        }

        public FormDto Recipe { get; set; }

        public async Task Submit()
        {
            // TODO: add proper owner id
            await commandHandler.Add(Models.Recipe.Create(
                Guid.NewGuid(),
                Guid.NewGuid(),
                Recipe.Name,
                new Uri(Recipe.ImageUrl ?? DefaultImageUrl),
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
