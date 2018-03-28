using Xamarin.Forms;
using SmartRecipes.Mobile.Views;
using SmartRecipes.Mobile.Extensions;
using System.Linq;

namespace SmartRecipes.Mobile.Pages
{
    public partial class ShoppingListItemsPage : ContentPage
    {
        public ShoppingListItemsPage(ShoppingListItemsViewModel viewModel, Store store)
        {
            InitializeComponent();

            BindingContext = viewModel;
            var cellViewModels = viewModel.Items.Select(i => new ShoppingListItemCellViewModel(i, store));

            ItemsListView.ItemTemplate = new DataTemplate<ShoppingListItemCell>();
            ItemsListView.ItemsSource = cellViewModels;
            AddItemsButton.Clicked += (s, e) => viewModel.NavigateToAddItemPage();
        }
    }
}
