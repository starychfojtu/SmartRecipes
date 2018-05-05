using SmartRecipes.Mobile.WriteModels;
using System;
using SmartRecipes.Mobile.ViewModels;
using System.Threading.Tasks;
using SmartRecipes.Mobile.Models;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

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
            Ingredients = ImmutableList.Create<IFoodstuffAmount>();
        }

        public FormDto Recipe { get; set; }

        public ImmutableList<IFoodstuffAmount> Ingredients { get; private set; }

        public async Task OpenAddIngredientDialog()
        {
            var newIngredient = FoodstuffAmount.Create(Guid.NewGuid(), null);
            UpdateIngredients(Ingredients.Add(newIngredient));
        }

        public async Task Submit()
        {
            var recipe = Models.Recipe.Create(
                Guid.NewGuid(),
                Guid.NewGuid(),
                Recipe.Name,
                new Uri(Recipe.ImageUrl ?? DefaultImageUrl),
                Recipe.PersonCount,
                Recipe.Text
            );
            var ingredients = Ingredients.Select(i => i.WithRecipe(recipe));
            await commandHandler.Add(recipe, ingredients);
        }

        private void UpdateIngredients(IEnumerable<IFoodstuffAmount> ingredients)
        {
            Ingredients = ingredients;
            RaisePropertyChanged(nameof(Ingredients));
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
