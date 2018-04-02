using Xamarin.Forms;
using FFImageLoading.Transformations;

namespace SmartRecipes.Mobile.Views
{
    public partial class FoodstuffCell : ViewCell
    {
        public FoodstuffCell()
        {
            InitializeComponent();

            Image.Transformations.Add(new CircleTransformation());

            MinusButton.Clicked += (s, e) => OnMinus();
            PlusButton.Clicked += (s, e) => OnPlus();
        }

        private FoodstuffCellViewModel ViewModel => (BindingContext as FoodstuffCellViewModel);

        private void OnMinus()
        {
            ViewModel.OnMinus?.Invoke();
        }

        private void OnPlus()
        {
            ViewModel.OnPlus.Invoke();
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            if (ViewModel != null)
            {
                NameLabel.Text = ViewModel.Foodstuff.Name;
                AmountLabel.Text = ViewModel.Amount.ToString(); // TODO: add amount needed
                Image.Source = ViewModel.Foodstuff.ImageUrl.AbsoluteUri;
                MinusButton.IsVisible = ViewModel.OnMinus != null;
            }
        }
    }
}
