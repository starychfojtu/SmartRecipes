using Xamarin.Forms;
using FFImageLoading.Transformations;
using System.Linq;
using FFImageLoading.Forms;

namespace SmartRecipes.Mobile.Views
{
    public partial class RecipeCell : ViewCell
    {
        public RecipeCell()
        {
            InitializeComponent();

            OtherButton.Clicked += (s, e) => OnOther();
            PlusButton.Clicked += (s, e) => OnPlus();
        }

        private RecipeCellViewModel ViewModel => (BindingContext as RecipeCellViewModel);

        private void OnOther()
        {
            ViewModel.OnOther?.Invoke();
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
                NameLabel.Text = ViewModel.Recipe.Name;
                OtherButton.IsVisible = ViewModel.OnOther != null;
                IngredientsStackLayout.Children.Clear();

                foreach (var ingredient in ViewModel.Recipe.Ingredients)
                {
                    var image = new CachedImage
                    {
                        HeightRequest = 32,
                        WidthRequest = 32,
                        VerticalOptions = LayoutOptions.Center,
                        Source = ingredient.Foodstuff.ImageUrl,
                        DownsampleToViewSize = true
                    };
                    image.Transformations.Add(new CircleTransformation());
                    IngredientsStackLayout.Children.Add(image);
                }
            }
        }
    }
}
