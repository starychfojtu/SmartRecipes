using Xamarin.Forms;
using SmartRecipes.Mobile.Views;
using SmartRecipes.Mobile.Extensions;

namespace SmartRecipes.Mobile.Pages
{
    public partial class ShoppingListItemsPage : ContentPage
    {
        public ShoppingListItemsPage(ShoppingListItemsViewModel viewModel)
        {
            InitializeComponent();

            BindingContext = viewModel;

            ItemsListView.ItemTemplate = new DataTemplate<ShoppingListItemCell>();
            ItemsListView.ItemsSource = viewModel.Items;
        }
    }
}
