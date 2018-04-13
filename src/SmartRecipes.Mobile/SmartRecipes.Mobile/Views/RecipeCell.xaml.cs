using Xamarin.Forms;
using System.Linq;

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

                // var thumbnails = ViewModel.Recipe.Ingredients.Select(i => Image.Thumbnail(i.Foodstuff.ImageUrl));
                // IngredientsStackLayout.Children.AddRange(thumbnails);
            }
        }
    }
}
