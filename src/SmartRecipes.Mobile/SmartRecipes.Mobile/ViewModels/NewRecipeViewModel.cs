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
using LanguageExt;

namespace SmartRecipes.Mobile
{
    public class NewRecipeViewModel : ViewModel
    {
        private readonly Database database;

        private const string DefaultImageUrl = "https://thumbs.dreamstime.com/z/empty-dish-14513513.jpg";

        private IImmutableDictionary<IFoodstuff, IAmount> ingredients;

        public NewRecipeViewModel(Database database)
        {
            this.database = database;
            Recipe = new FormDto();
            ingredients = ImmutableDictionary.Create<IFoodstuff, IAmount>();
        }

        public FormDto Recipe { get; set; }

        public IEnumerable<FoodstuffAmountCellViewModel> Ingredients
        {
            get { return ingredients.Select(kvp => ToViewModel(kvp.Key, kvp.Value)); }
        }

        public async Task OpenAddIngredientDialog()
        {
            var foodstuffs = await Navigation.SelectFoodstuffDialog();
            var newFoodstuffs = foodstuffs.Where(f => !ingredients.ContainsKey(f)).Select(f => new KeyValuePair<IFoodstuff, IAmount>(f, f.BaseAmount));
            var newIngredients = ingredients.AddRange(newFoodstuffs);

            UpdateIngredients(newIngredients);
        }

        public async Task Submit()
        {
            var recipe = Models.Recipe.Create(
                CurrentAccount.ToSome(),
                Recipe.Name,
                new Uri(Recipe.ImageUrl ?? DefaultImageUrl),
                Recipe.PersonCount,
                Recipe.Text
            );
            var recipeIngredients = ingredients.Select(kvp => IngredientAmount.Create(recipe.ToSome(), kvp.Key.ToSome(), kvp.Value));

            await MyRecipesHandler.Add(database, recipe, recipeIngredients);
        }

        private Task UpdateIngredient(IFoodstuff foodstuff, Func<IAmount, IAmount, Option<IAmount>> action)
        {
            var newAmount = action(ingredients[foodstuff], foodstuff.AmountStep).IfNone(foodstuff.BaseAmount);
            var newIngredients = ingredients.SetItem(foodstuff, newAmount);

            UpdateIngredients(newIngredients);
            return Task.CompletedTask;
        }

        private void UpdateIngredients(IImmutableDictionary<IFoodstuff, IAmount> newIngredients)
        {
            ingredients = newIngredients;
            RaisePropertyChanged(nameof(Ingredients));
        }

        private FoodstuffAmountCellViewModel ToViewModel(IFoodstuff foodstuff, IAmount amount)
        {
            return new FoodstuffAmountCellViewModel(
                foodstuff.ToSome(),
                amount.ToSome(),
                () => UpdateIngredient(foodstuff, (a1, a2) => Amount.Add(a1, a2)),
                () => UpdateIngredient(foodstuff, (a1, a2) => Amount.Substract(a1, a2))
            );
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
