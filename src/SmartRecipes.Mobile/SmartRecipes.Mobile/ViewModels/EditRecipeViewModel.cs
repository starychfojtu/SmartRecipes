using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using LanguageExt;
using SmartRecipes.Mobile.Infrastructure;
using SmartRecipes.Mobile.Models;
using SmartRecipes.Mobile.Services;
using SmartRecipes.Mobile.WriteModels;
using Xamarin.Forms;
using static LanguageExt.Prelude;

namespace SmartRecipes.Mobile.ViewModels
{
    public enum EditRecipeMode
    {
        New,
        Edit
    }

    public class EditRecipeViewModel : ViewModel
    {
        private readonly Enviroment enviroment;

        public EditRecipeViewModel(Enviroment enviroment)
        {
            this.enviroment = enviroment;
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
            var getIngredients = fun((IRecipe r) => Ingredients.Select(kvp => IngredientAmount.Create(r, kvp.Key, kvp.Value)));
            var submitTask = Mode == EditRecipeMode.New
                ? CreateRecipe(getIngredients)
                : UpdateRecipe(getIngredients);

            await submitTask;
            await Application.Current.MainPage.Navigation.PopAsync();
        }

        public async Task CreateRecipe(Func<IRecipe, IEnumerable<IIngredientAmount>> getIngredients)
        {
            var recipe = Models.Recipe.Create(
                CurrentAccount,
                Recipe.Name,
                Optional(Recipe.ImageUrl).Map(url => new Uri(url)),
                Recipe.PersonCount,
                Recipe.Text
            );

            await MyRecipesHandler.Add(enviroment, recipe, getIngredients(recipe));
        }

        public async Task UpdateRecipe(Func<IRecipe, IEnumerable<IIngredientAmount>> getIngredients)
        {
            var recipe = Models.Recipe.Create(
                Recipe.Id.Value,
                CurrentAccount.Id,
                Recipe.Name,
                Optional(Recipe.ImageUrl).Map(url => new Uri(url)),
                Recipe.PersonCount,
                Recipe.Text
            );

            await MyRecipesHandler.Update(enviroment, recipe, getIngredients(recipe));
        }

        private Task<Unit> ChangeAmount(IFoodstuff foodstuff, Func<IAmount, IAmount, Option<IAmount>> action)
        {
            var newAmount = action(Ingredients[foodstuff], foodstuff.AmountStep).IfNone(foodstuff.BaseAmount);
            var newIngredients = Ingredients.SetItem(foodstuff, newAmount);

            return Task.FromResult(UpdateIngredients(newIngredients));
        }
        
        private Task<Option<UserMessage>> DeleteIngredient(IFoodstuff foodstuff)
        {
            return Task.FromResult(UpdateIngredients(Ingredients.Remove(foodstuff))).Map(_ => Option<UserMessage>.None);
        }

        private Unit UpdateIngredients(IImmutableDictionary<IFoodstuff, IAmount> newIngredients)
        {
            Ingredients = newIngredients;
            RaisePropertyChanged(nameof(Ingredients));
            RaisePropertyChanged(nameof(IngredientViewModels));
            return Unit.Default;
        }

        private FoodstuffAmountCellViewModel ToViewModel(IFoodstuff foodstuff, IAmount amount)
        {
            return new FoodstuffAmountCellViewModel(
                foodstuff,
                amount,
                None,
                () => ChangeAmount(foodstuff, (a1, a2) => Amount.Add(a1, a2)),
                () => ChangeAmount(foodstuff, (a1, a2) => Amount.Substract(a1, a2)),
                new UserAction<Unit>(_ => DeleteIngredient(foodstuff), Icon.Delete(), 1)
            );
        }

        public class FormDto
        {
            public Guid? Id { get; set; }

            public string Name { get; set; }

            public string ImageUrl { get; set; }

            public int PersonCount { get; set; }

            public string Text { get; set; }
        }
    }
}
