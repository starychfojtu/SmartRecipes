using Xamarin.Forms;
using SmartRecipes.Mobile.Views;

namespace SmartRecipes.Mobile.Pages
{
    public partial class ShoppingListItemsPage : ContentPage
    {
        public ShoppingListItemsPage(ShoppingListItemsViewModel viewModel)
        {
            InitializeComponent();

            BindingContext = viewModel;

            var itemCell = new DataTemplate(typeof(ShoppingListItemCell));
            itemCell.SetBinding(ShoppingListItemCell.ItemProperty, $"");

            ItemsListView.ItemTemplate = itemCell;
            ItemsListView.ItemsSource = viewModel.Items;
        }
    }
}
