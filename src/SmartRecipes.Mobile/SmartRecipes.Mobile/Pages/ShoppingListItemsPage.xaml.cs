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
            itemCell.SetBinding(ShoppingListItemCell.NameProperty, $"{nameof(ShoppingListItem.Foodstuff)}.{nameof(Foodstuff.Name)}");
            itemCell.SetBinding(ShoppingListItemCell.AmountProperty, nameof(ShoppingListItem.Amount));
            itemCell.SetBinding(ShoppingListItemCell.ImageUrlProperty, $"{nameof(ShoppingListItem.Foodstuff)}.{nameof(Foodstuff.ImageUrl)}");

            ItemsListView.ItemTemplate = itemCell;
            ItemsListView.ItemsSource = viewModel.Items;
        }
    }
}
