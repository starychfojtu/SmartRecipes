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
using Xamarin.Forms;

namespace SmartRecipes.Mobile
{
    public enum EditRecipeMode
    {
        New,
        Edit
    }

    public class EditRecipeViewModel : ViewModel
    {
        private readonly Database database;

        private const string DefaultImageUrl = "https://thumbs.dreamstime.com/z/empty-dish-14513513.jpg";

        public EditRecipeViewModel(Database database)
        {
            this.database = database;
            Recipe = new FormDto();
            Ingredients = ImmutableDictionary.Create<IFoodstuff, IAmount>();
            Mode = EditRecipeMode.New;
        }

        public EditRecipeMode Mode { get; set; }

        public FormDto Recipe { get; set; }

        public IImmutableDictionary<IFoodstuff, IAmount> Ingredients { get; set; }

        public IEnumerable<FoodstuffAmountCellViewModel> IngredientViewModels
        {
            get { return Ingredients.Select(kvp => ToViewModel(kvp.Key, kvp.Value)); }
        }

        public async Task OpenAddIngredientDialog()
        {
            var foodstuffs = await Navigation.SelectFoodstuffDialog();
            var newFoodstuffs = foodstuffs.Where(f => !Ingredients.ContainsKey(f)).Select(f => new KeyValuePair<IFoodstuff, IAmount>(f, f.BaseAmount));
            var newIngredients = Ingredients.AddRange(newFoodstuffs);

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
            var recipeIngredients = Ingredients.Select(kvp => IngredientAmount.Create(recipe.ToSome(), kvp.Key.ToSome(), kvp.Value));

            await MyRecipesHandler.Add(database, recipe, recipeIngredients);
            await Application.Current.MainPage.Navigation.PopModalAsync();
        }

        private Task UpdateIngredient(IFoodstuff foodstuff, Func<IAmount, IAmount, Option<IAmount>> action)
        {
            var newAmount = action(Ingredients[foodstuff], foodstuff.AmountStep).IfNone(foodstuff.BaseAmount);
            var newIngredients = Ingredients.SetItem(foodstuff, newAmount);

            UpdateIngredients(newIngredients);
            return Task.CompletedTask;
        }

        private void UpdateIngredients(IImmutableDictionary<IFoodstuff, IAmount> newIngredients)
        {
            Ingredients = newIngredients;
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
