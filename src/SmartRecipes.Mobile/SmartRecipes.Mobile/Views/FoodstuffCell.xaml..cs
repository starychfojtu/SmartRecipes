using Xamarin.Forms;
using FFImageLoading.Transformations;
using System.Threading.Tasks;
using SmartRecipes.Mobile.ViewModels;

namespace SmartRecipes.Mobile.Views
{
    public partial class FoodstuffCell : ViewCell
    {
        private FoodstuffCellViewModel CachedViewModel;

        public FoodstuffCell()
        {
            InitializeComponent();

            Image.Transformations.Add(new CircleTransformation());

            MinusButton.Clicked += async (s, e) => await OnMinus();
            PlusButton.Clicked += async (s, e) => await OnPlus();
        }

        private FoodstuffCellViewModel ViewModel => (BindingContext as FoodstuffCellViewModel);

        private async Task OnMinus()
        {
            await ViewModel.OnMinus?.Invoke();
        }

        private async Task OnPlus()
        {
            await ViewModel.OnPlus.Invoke();
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            if (ViewModel != null)
            {
                NameLabel.Text = ViewModel.Foodstuff.Name;
                AmountLabel.Text = ViewModel.Amount.ToString(); // TODO: add amount needed
                MinusButton.IsVisible = ViewModel.OnMinus != null;

                var imageUrl = ViewModel.Foodstuff.ImageUrl;
                if (imageUrl != CachedViewModel?.Foodstuff.ImageUrl)
                {
                    Image.Source = imageUrl.AbsoluteUri;
                }

                CachedViewModel = ViewModel;
            }
        }
    }
}
