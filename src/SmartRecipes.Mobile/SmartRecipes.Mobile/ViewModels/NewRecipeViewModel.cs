using SmartRecipes.Mobile.WriteModels;
using System;
using SmartRecipes.Mobile.ViewModels;
using System.Threading.Tasks;
using System.Collections.Immutable;
using System.Linq;
using SmartRecipes.Mobile.ReadModels.Dto;
using System.Collections.Generic;
using SmartRecipes.Mobile.Services;
using LanguageExt.SomeHelp;
using SmartRecipes.Mobile.Models;

namespace SmartRecipes.Mobile
{
    public class NewRecipeViewModel : ViewModel
    {
        private readonly MyRecipesHandler recipeHandler;

        private readonly UserHandler userHandler;

        private const string DefaultImageUrl = "https://thumbs.dreamstime.com/z/empty-dish-14513513.jpg";

        private IImmutableList<Ingredient> ingredients;

        public NewRecipeViewModel(MyRecipesHandler recipeHandler, UserHandler userHandler)
        {
            this.recipeHandler = recipeHandler;
            this.userHandler = userHandler;
            Recipe = new FormDto();
            ingredients = ImmutableList.Create<Ingredient>();
        }

        public FormDto Recipe { get; set; }

        public IEnumerable<IngredientCellViewModel> Ingredients
        {
            get { return ingredients.Select(i => new IngredientCellViewModel(i, null, null)); }
        }

        public async Task OpenAddIngredientDialog()
        {
            var foodstuffs = await Navigation.SelectFoodstuffDialog();
            // TODO: refactor creating of objects in whole solution
            var newIngredients = foodstuffs.Select(f => new Ingredient(
                f.ToSome(),
                FoodstuffAmount.CreateForRecipe(Guid.NewGuid(), Guid.Empty, f.Id, f.BaseAmount).ToSome()
            ));
            UpdateIngredients(ingredients.Concat(newIngredients).ToImmutableList());
        }

        public async Task Submit()
        {
            var recipe = Models.Recipe.Create(
                Guid.NewGuid(),
                userHandler.CurrentAccount.Id, // TODO: this should probably set recipehandler, not viewModel
                Recipe.Name,
                new Uri(Recipe.ImageUrl ?? DefaultImageUrl),
                Recipe.PersonCount,
                Recipe.Text
            );
            var recipeIngredients = ingredients.Select(i => i.FoodstuffAmount.WithRecipe(recipe));
            await recipeHandler.Add(recipe, recipeIngredients);
        }

        private void UpdateIngredients(IImmutableList<Ingredient> newIngredients)
        {
            ingredients = newIngredients;
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
