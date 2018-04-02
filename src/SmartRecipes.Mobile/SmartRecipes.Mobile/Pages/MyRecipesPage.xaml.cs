using Xamarin.Forms;
using SmartRecipes.Mobile.Views;
using SmartRecipes.Mobile.Extensions;

namespace SmartRecipes.Mobile
{
    public partial class MyRecipesPage : ContentPage
    {
        public MyRecipesPage(MyRecipesViewModel viewModel)
        {
            InitializeComponent();

            BindingContext = viewModel;

            RecipeListView.ItemTemplate = new DataTemplate<RecipeCell>();
            RecipeListView.SetBinding(ItemsView<Cell>.ItemsSourceProperty, nameof(viewModel.Recipes));
        }
    }
}
