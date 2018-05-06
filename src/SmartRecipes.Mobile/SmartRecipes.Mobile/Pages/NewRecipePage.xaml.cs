using SmartRecipes.Mobile.Extensions;
using SmartRecipes.Mobile.Views;
using Xamarin.Forms;

namespace SmartRecipes.Mobile.Pages
{
    public partial class NewRecipePage : ContentPage
    {
        public NewRecipePage(NewRecipeViewModel viewModel)
        {
            InitializeComponent();

            BindingContext = viewModel;

            viewModel.BindText(NameEntry, vm => vm.Recipe.Name);
            viewModel.BindText(ImageUrlEntry, vm => vm.Recipe.ImageUrl);
            viewModel.BindText(PersonCountEntry, vm => vm.Recipe.PersonCount);
            viewModel.Bind(TextEditor, Entry.TextProperty, vm => vm.Recipe.Text);

            IngredientsListView.ItemTemplate = new DataTemplate<FoodstuffSearchCell>();
            IngredientsListView.SetBinding(ItemsView<Cell>.ItemsSourceProperty, nameof(viewModel.Ingredients));

            AddIngredientButton.Clicked += async (s, e) => await viewModel.OpenAddIngredientDialog();
            SubmitButton.Clicked += async (s, e) => await viewModel.Submit();
        }
    }
}
