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

            ItemsListView.ItemTemplate = new DataTemplate<FoodstuffCell>();
            ItemsListView.SetBinding(ItemsView<Cell>.ItemsSourceProperty, nameof(viewModel.GetItems));

            AddItemsButton.Clicked += (s, e) => viewModel.NavigateToAddItemPage();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            (BindingContext as ShoppingListItemsViewModel).Refresh();
        }
    }
}
