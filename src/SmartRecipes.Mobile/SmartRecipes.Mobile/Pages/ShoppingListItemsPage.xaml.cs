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

            ItemsListView.ItemTemplate = new DataTemplate<FoodstuffAmountCell>();
            viewModel.BindValue(ItemsListView, ItemsView<Cell>.ItemsSourceProperty, vm => vm.ShoppingListItems);

            AddItemsButton.Clicked += async (s, e) => await viewModel.OpenAddFoodstuffDialog();
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            await (BindingContext as ShoppingListItemsViewModel).Refresh();
        }
    }
}
