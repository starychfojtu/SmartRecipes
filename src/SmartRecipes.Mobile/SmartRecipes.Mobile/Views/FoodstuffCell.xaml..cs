using Xamarin.Forms;
using FFImageLoading.Transformations;
using System.Threading.Tasks;

namespace SmartRecipes.Mobile.Views
{
    public partial class FoodstuffCell : ViewCell
    {
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
                Image.Source = ViewModel.Foodstuff.ImageUrl.AbsoluteUri;
                MinusButton.IsVisible = ViewModel.OnMinus != null;
            }
        }
    }
}
