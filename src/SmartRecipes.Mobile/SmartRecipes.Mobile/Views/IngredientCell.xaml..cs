using Xamarin.Forms;
using FFImageLoading.Transformations;
using System.Threading.Tasks;
using SmartRecipes.Mobile.ViewModels;

namespace SmartRecipes.Mobile.Views
{
    public partial class IngredientCell : ViewCell
    {
        private IngredientCellViewModel CachedViewModel;

        public IngredientCell()
        {
            InitializeComponent();

            Image.Transformations.Add(new CircleTransformation());

            MinusButton.Clicked += async (s, e) => await ViewModel.OnMinus?.Invoke();
            PlusButton.Clicked += async (s, e) => await ViewModel.OnPlus.Invoke();
        }

        private IngredientCellViewModel ViewModel => (BindingContext as IngredientCellViewModel);

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            if (ViewModel != null)
            {
                NameLabel.Text = ViewModel.Ingredient.Foodstuff.Name;
                AmountLabel.Text = ViewModel.Ingredient.Amount.ToString(); // TODO: add amount needed
                MinusButton.IsVisible = ViewModel.OnMinus != null;

                var imageUrl = ViewModel.Ingredient.Foodstuff.ImageUrl;
                if (imageUrl != CachedViewModel?.Ingredient.Foodstuff.ImageUrl)
                {
                    // TODO: resolve disposed bitmaps
                    // Image.Source = imageUrl.AbsoluteUri;
                }

                CachedViewModel = ViewModel;
            }
        }
    }
}
