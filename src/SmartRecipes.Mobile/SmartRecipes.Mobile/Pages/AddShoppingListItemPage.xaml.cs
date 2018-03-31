using Xamarin.Forms;
using SmartRecipes.Mobile.Views;
using SmartRecipes.Mobile.Extensions;

namespace SmartRecipes.Mobile.Pages
{
    public partial class AddShoppingListItemPage : ContentPage
    {
        public AddShoppingListItemPage(AddShoppingListItemViewModel viewModel)
        {
            InitializeComponent();

            BindingContext = viewModel;

            Search.TextChanged += (s, e) => viewModel.Search(e.NewTextValue);
            SearchedItemsListView.ItemTemplate = new DataTemplate<FoodstuffCell>();
            SearchedItemsListView.SetBinding(ItemsView<Cell>.ItemsSourceProperty, nameof(viewModel.SearchResult));
        }
    }
}
