using Xamarin.Forms;
using SmartRecipes.Mobile.Views;
using SmartRecipes.Mobile.Extensions;
using SmartRecipes.Mobile.ViewModels;

namespace SmartRecipes.Mobile.Pages
{
    public partial class ShoppingListItemsPage : ContentPage
    {
        public ShoppingListItemsPage(ShoppingListItemsViewModel viewModel)
        {
            InitializeComponent();

            BindingContext = viewModel;

            ItemsListView.ItemTemplate = new DataTemplate<FoodstuffCell>();
            ItemsListView.SetBinding(ItemsView<Cell>.ItemsSourceProperty, nameof(viewModel.Items));

            AddItemsButton.Clicked += async (s, e) => await viewModel.AddItem();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            (BindingContext as ShoppingListItemsViewModel).Refresh();
        }
    }
}
