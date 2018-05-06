using Xamarin.Forms;
using SmartRecipes.Mobile.Views;
using SmartRecipes.Mobile.Extensions;
using SmartRecipes.Mobile.ViewModels;
using System;
using SmartRecipes.Mobile.Models;
using System.Collections.Generic;

namespace SmartRecipes.Mobile.Pages
{
    public partial class FoodstuffSearchPage : ContentPage
    {
        public FoodstuffSearchPage(FoodstuffSearchViewModel viewModel)
        {
            InitializeComponent();

            BindingContext = viewModel;

            Search.TextChanged += (s, e) => viewModel.Search(e.NewTextValue);
            SearchedItemsListView.ItemTemplate = new DataTemplate<FoodstuffSearchCell>();
            SearchedItemsListView.SetBinding(ItemsView<Cell>.ItemsSourceProperty, nameof(viewModel.SearchResult));
        }

        public event EventHandler<FoodstuffSelectedArgs> SelectingEnded;

        protected override void OnDisappearing()
        {
            var vm = BindingContext as FoodstuffSearchViewModel;
            SelectingEnded?.Invoke(this, new FoodstuffSelectedArgs(vm.Selected));

            base.OnDisappearing();
        }
    }

    public class FoodstuffSelectedArgs : EventArgs
    {
        public FoodstuffSelectedArgs(IEnumerable<IFoodstuff> selected)
        {
            Selected = selected;
        }

        public IEnumerable<IFoodstuff> Selected { get; }
    }
}
