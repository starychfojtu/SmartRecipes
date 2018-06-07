using Xamarin.Forms;
using SmartRecipes.Mobile.ViewModels;
using FFImageLoading.Transformations;

namespace SmartRecipes.Mobile.Views
{
    public partial class FoodstuffAmountCell : ViewCell
    {
        public FoodstuffAmountCell()
        {
            InitializeComponent();

            Image.Transformations.Add(new CircleTransformation());

            MinusButton.Clicked += async (s, e) => await ViewModel.OnMinus?.Invoke();
            PlusButton.Clicked += async (s, e) => await ViewModel.OnPlus.Invoke();
        }

        private FoodstuffAmountCellViewModel ViewModel => (BindingContext as FoodstuffAmountCellViewModel);

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            if (ViewModel != null)
            {
                NameLabel.Text = ViewModel.Foodstuff.Name;
                AmountLabel.Text = ViewModel.Amount.ToString(); // TODO: add amount needed
                MinusButton.IsVisible = ViewModel.OnMinus != null;
                Image.Source = ViewModel.Foodstuff.ImageUrl;
            }
        }
    }
}
