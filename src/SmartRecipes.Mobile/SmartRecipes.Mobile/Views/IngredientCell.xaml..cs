using Xamarin.Forms;
using SmartRecipes.Mobile.ViewModels;
using FFImageLoading.Transformations;

namespace SmartRecipes.Mobile.Views
{
    public partial class IngredientCell : ViewCell
    {
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
                Image.Source = ViewModel.Ingredient.Foodstuff.ImageUrl;
            }
        }
    }
}
