using SmartRecipes.Mobile.WriteModels;
using System;
using SmartRecipes.Mobile.ViewModels;
using System.Threading.Tasks;
using System.Collections.Immutable;
using System.Linq;
using System.Collections.Generic;
using SmartRecipes.Mobile.Services;
using LanguageExt.SomeHelp;
using SmartRecipes.Mobile.Models;

namespace SmartRecipes.Mobile
{
    public class NewRecipeViewModel : ViewModel
    {
        private readonly Database database;

        private const string DefaultImageUrl = "https://thumbs.dreamstime.com/z/empty-dish-14513513.jpg";

        private IImmutableList<IngredientDto> ingredients;

        public NewRecipeViewModel(Database database)
        {
            this.database = database;
            Recipe = new FormDto();
            ingredients = ImmutableList.Create<IngredientDto>();
        }

        public FormDto Recipe { get; set; }

        public IEnumerable<FoodstuffAmountCellViewModel> Ingredients
        {
            get { return ingredients.Select(i => new FoodstuffAmountCellViewModel(i.Foodstuff.ToSome(), i.Amount.ToSome(), null, null)); }
        }

        public async Task OpenAddIngredientDialog()
        {
            var foodstuffs = await Navigation.SelectFoodstuffDialog();
            var newIngredients = foodstuffs.Select(f => new IngredientDto(f, f.BaseAmount));

            UpdateIngredients(ingredients.Concat(newIngredients).ToImmutableList());
        }

        public async Task Submit()
        {
            // TODO: this should happen recipehandler
            var recipeId = Guid.NewGuid();
            var recipe = Models.Recipe.Create(
                recipeId,
                CurrentAccount,
                Recipe.Name,
                new Uri(Recipe.ImageUrl ?? DefaultImageUrl),
                Recipe.PersonCount,
                Recipe.Text
            );
            var recipeIngredients = ingredients.Select(i => IngredientAmount.Create(Guid.NewGuid(), recipeId, i.Foodstuff.Id, i.Amount));
            await MyRecipesHandler.Add(database, recipe, recipeIngredients);
        }

        private void UpdateIngredients(IImmutableList<IngredientDto> newIngredients)
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

        public class IngredientDto
        {
            public IngredientDto(IFoodstuff foodstuff, IAmount amount)
            {
                Foodstuff = foodstuff;
                Amount = amount;
            }

            public IFoodstuff Foodstuff { get; }

            public IAmount Amount { get; }
        }
    }
}
