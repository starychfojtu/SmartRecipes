using Xamarin.Forms;
using SmartRecipes.Mobile.Views;
using SmartRecipes.Mobile.Extensions;
using SmartRecipes.Mobile.ViewModels;

namespace SmartRecipes.Mobile.Pages
{
    public partial class FoodstuffSearchPage : ContentPage
    {
        public FoodstuffSearchPage(FoodstuffSearchViewModel viewModel)
        {
            InitializeComponent();

            BindingContext = viewModel;

            Search.TextChanged += (s, e) => viewModel.Search(e.NewTextValue);
            SearchedItemsListView.ItemTemplate = new DataTemplate<FoodstuffCell>();
            SearchedItemsListView.SetBinding(ItemsView<Cell>.ItemsSourceProperty, nameof(viewModel.SearchResult));
        }
    }
}
