using SmartRecipes.Mobile.Extensions;
using SmartRecipes.Mobile.ViewModels;
using SmartRecipes.Mobile.Views;
using Xamarin.Forms;

namespace SmartRecipes.Mobile.Pages
{
    public partial class ShoppingListRecipesPage : ContentPage
    {
        public ShoppingListRecipesPage(ShoppingListRecipesViewModel viewModel)
        {
            InitializeComponent();
            
            BindingContext = viewModel;
            
            RecipeListView.ItemTemplate = new DataTemplate<RecipeCell>();
            RecipeListView.SetBinding(ItemsView<Cell>.ItemsSourceProperty, nameof(viewModel.Recipes));
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await (BindingContext as ShoppingListRecipesViewModel).InitializeAsync();
        }
    }
}
