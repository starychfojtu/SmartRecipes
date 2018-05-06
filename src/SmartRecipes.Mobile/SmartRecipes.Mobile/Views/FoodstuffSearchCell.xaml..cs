using Xamarin.Forms;
using FFImageLoading.Transformations;
using SmartRecipes.Mobile.ViewModels;

namespace SmartRecipes.Mobile.Views
{
    public partial class FoodstuffSearchCell : ViewCell
    {
        private FoodstuffSearchCellViewModel CachedViewModel;

        public FoodstuffSearchCell()
        {
            InitializeComponent();

            Image.Transformations.Add(new CircleTransformation());

            PlusButton.Clicked += (s, e) => ViewModel.OnSelected();
        }

        private FoodstuffSearchCellViewModel ViewModel => (BindingContext as FoodstuffSearchCellViewModel);

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            if (ViewModel != null)
            {
                NameLabel.Text = ViewModel.Foodstuff.Name;
                AmountLabel.Text = ViewModel.Foodstuff.BaseAmount.ToString();

                var imageUrl = ViewModel.Foodstuff.ImageUrl;
                if (imageUrl != CachedViewModel?.Foodstuff.ImageUrl)
                {
                    // TODO: resolve disposed bitmaps
                    // Image.Source = imageUrl.AbsoluteUri;
                }

                CachedViewModel = ViewModel;
            }
        }
    }
}
