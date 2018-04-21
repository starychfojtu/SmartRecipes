using Xamarin.Forms;
using System.Linq;
using SmartRecipes.Mobile.ViewModels;

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
                var recipe = ViewModel.Recipe;
                NameLabel.Text = recipe.Name;
                OtherButton.IsVisible = ViewModel.OnOther != null;
                IngredientsStackLayout.Children.Clear();

                // TODO: in future versions
                //var thumbnails = ingredients.Select(i => Image.Thumbnail(i.Value.Foodstuff.ImageUrl));
                //IngredientsStackLayout.Children.AddRange(thumbnails);
            }
        }
    }
}
