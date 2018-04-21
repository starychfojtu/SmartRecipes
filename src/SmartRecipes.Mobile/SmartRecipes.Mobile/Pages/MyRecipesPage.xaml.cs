using Xamarin.Forms;
using SmartRecipes.Mobile.Views;
using SmartRecipes.Mobile.Extensions;
using SmartRecipes.Mobile.ViewModels;

namespace SmartRecipes.Mobile.Pages
{
    public partial class MyRecipesPage : ContentPage
    {
        public MyRecipesPage(MyRecipesViewModel viewModel)
        {
            InitializeComponent();

            BindingContext = viewModel;

            // TODO: Update when OnAppearing
            RecipeListView.ItemTemplate = new DataTemplate<RecipeCell>();
            RecipeListView.SetBinding(ItemsView<Cell>.ItemsSourceProperty, nameof(viewModel.Recipes));

            AddRecipeButton.Clicked += async (s, e) => await viewModel.AddRecipe();
        }
    }
}
