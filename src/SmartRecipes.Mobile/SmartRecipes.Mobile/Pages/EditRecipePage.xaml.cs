﻿using SmartRecipes.Mobile.Extensions;
using SmartRecipes.Mobile.Views;
using Xamarin.Forms;

namespace SmartRecipes.Mobile.Pages
{
    public partial class EditRecipePage : ContentPage
    {
        public EditRecipePage(EditRecipeViewModel viewModel)
        {
            InitializeComponent();

            BindingContext = viewModel;

            viewModel.BindText(NameEntry, vm => vm.Recipe.Name);
            viewModel.BindText(ImageUrlEntry, vm => vm.Recipe.ImageUrl);
            viewModel.BindText(PersonCountEntry, vm => vm.Recipe.PersonCount);
            viewModel.Bind(TextEditor, Editor.TextProperty, vm => vm.Recipe.Text);

            IngredientsListView.ItemTemplate = new DataTemplate<FoodstuffAmountCell>();
            viewModel.Bind(IngredientsListView, ItemsView<Cell>.ItemsSourceProperty, vm => vm.IngredientViewModels);

            AddIngredientButton.Clicked += async (s, e) => await viewModel.OpenAddIngredientDialog();
            SubmitButton.Clicked += async (s, e) => await viewModel.Submit();
        }
    }
}
